using System;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Surfaces;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class Wurm : MonoBehaviour
{
    [SerializeField] private GameObject preNode;
    [SerializeField] public InputActionAsset controls;
    [SerializeField] private Wurm preWurm;
    
    [HideInInspector] [SerializeField] private Spline spline;
    [HideInInspector] [SerializeField] private SplineContainer splineContainer;
    [HideInInspector] [SerializeField] private SplineExtrude splineExtrude;
    [HideInInspector] [SerializeField] private SplineInstantiate splineInstantiate;

    [HideInInspector] [SerializeField] private MeshFilter meshFilter;
    [HideInInspector] [SerializeField] private MeshRenderer meshRenderer;
    [HideInInspector] [SerializeField] private MeshCollider meshCollider;
    [HideInInspector] [SerializeField] private RayInteractable rayInteractable;
    [HideInInspector] [SerializeField] private ColliderSurface colliderSurface;
    [HideInInspector] [SerializeField] private Rigidbody rigidbodySpline;
    [HideInInspector] [SerializeField] private Grabbable grabbable;
    [HideInInspector] [SerializeField] private HandGrabInteractable handGrab;
    [HideInInspector] [SerializeField] private GrabInteractable grabInteractable;
    
    [HideInInspector] [SerializeField] private GameObject parentNode;
    [HideInInspector] [SerializeField] private GameObject[] nodes;

    
    [HideInInspector] [SerializeField] private bool selected;
    
    [HideInInspector] [SerializeField] private bool enableNodePlacement;
    
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Material nullMaterial;
    
    [SerializeField] private GameObject spherePrefab;

    private void Awake()
    {
        Setup();
    }

    public void Start()
    {
        var debugActionMap = controls.FindActionMap("Debug");
        
        var nodePlacement = debugActionMap.FindAction("NodePlacement");
        nodePlacement.performed += PlaceNode;
    }
    
    /// <summary>
    /// Für den Fall, das manche Componenten bereits existieren im Gameobject und andere noch nicht.
    /// </summary>

    private void Setup()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.materials[0] = nullMaterial;
        if (meshRenderer.materials[1] == null)
        {
            var materials = meshRenderer.materials;
            materials[1] = new Material(Shader.Find(new String("Universal Render Pipeline/Lit")));
            meshRenderer.materials = materials;
        }
        
        meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        
        splineContainer = gameObject.GetComponent<SplineContainer>();
        if (splineContainer == null)
            splineContainer = gameObject.AddComponent<SplineContainer>();
        spline = splineContainer.Spline;
        
        splineExtrude = gameObject.GetComponent<SplineExtrude>();
        if (splineExtrude == null)
            splineExtrude = gameObject.AddComponent<SplineExtrude>();
        splineExtrude.Container = splineContainer;
        splineExtrude.RebuildOnSplineChange = true;
        splineExtrude.SegmentsPerUnit = 20;
        splineExtrude.Radius = 0.05f;
        
        splineInstantiate = gameObject.GetComponent<SplineInstantiate>();
        if (splineInstantiate == null)
            splineInstantiate = gameObject.AddComponent<SplineInstantiate>();
        splineInstantiate.Container = splineContainer;
        splineInstantiate.enabled = false;
        
        meshFilter.mesh = new Mesh();
        meshCollider = gameObject.GetComponent<MeshCollider>();
        if (meshCollider == null)
            meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = meshFilter.mesh;
        
        colliderSurface = gameObject.GetComponent<ColliderSurface>();
        if (colliderSurface == null)
            colliderSurface = gameObject.AddComponent<ColliderSurface>();
        colliderSurface.InjectCollider(meshCollider);
        
        rayInteractable = gameObject.GetComponent<RayInteractable>();
        if (rayInteractable == null)
            rayInteractable = gameObject.AddComponent<RayInteractable>();
        rayInteractable.InjectSurface(colliderSurface);
        
        var interactableEventWrapper = gameObject.GetComponent<InteractableUnityEventWrapper>();
        interactableEventWrapper.InjectAllInteractableUnityEventWrapper(rayInteractable);               
        interactableEventWrapper.enabled = true;
        
        rigidbodySpline = gameObject.GetComponent<Rigidbody>();
        if (rigidbodySpline == null)
            rigidbodySpline = gameObject.AddComponent<Rigidbody>();
        rigidbodySpline.useGravity = false;
        rigidbodySpline.isKinematic = true;
        
        grabbable = gameObject.GetComponentInChildren<Grabbable>();
        if (grabbable == null)
            grabbable = gameObject.AddComponent<Grabbable>();
        grabbable.TransferOnSecondSelection = true;
        grabbable.InjectOptionalRigidbody(rigidbodySpline);
        
        handGrab = gameObject.GetComponentInChildren<HandGrabInteractable>();
        if (handGrab == null)
            handGrab = gameObject.AddComponent<HandGrabInteractable>();
        handGrab.InjectRigidbody(rigidbodySpline);
        handGrab.HandAlignment = HandAlignType.None;
        
        grabInteractable = gameObject.GetComponentInChildren<GrabInteractable>();
        if (grabInteractable == null)
            grabInteractable = gameObject.AddComponent<GrabInteractable>();
        grabInteractable.UseClosestPointAsGrabSource = false;
        grabInteractable.InjectRigidbody(rigidbodySpline);
    }
    
    /// <summary>
    /// eine Random generierung eines Wurmes
    /// </summary>

    private void Generate()
    {
        gameObject.SetActive(true);
        NodePlacementMode(false);
        DeleteNodes();
        spline.Clear();
        SetRandomSplineNodes();
        SetRandomRadius();
        //SetRandomPosition();
        SetRandomColor();
        CreateNodes();
    }

    public void OnHoverEnter()
    {
    }

    public void OnHoverExit()
    {
    }

    public void OnSelect()
    {
        SetMaterial(new Material(Shader.Find(new String("Universal Render Pipeline/Lit")))
        {
            color = Color.red
        });
    }

    public void OnUnselect() { Debug.Log("Unselect"); }
    
    public void OnButtonClick() { Generate(); }

    /// <summary>
    /// Die Nodes werden erzeugt.
    /// </summary>
    public void CreateNodes()
    {
        if (parentNode == null)
        {
            CreateParentNode();
        }

        if (nodes == null || nodes.Length == 0 && spline.ToArray().Length > 0)
        {
            var count = 0;
            nodes = new GameObject[spline.ToArray().Length];
            // Auf jedem Knot des Splines wird ein Node Gameobject erzeugt, damit man eine Visualisierung der Nodes hat
            foreach (var knot in spline.ToArray())
            {
                Debug.Log(count + " Knoten: " + knot);
                Vector3 knotposition = knot.Position;
                knotposition += transform.position;
                nodes[count] = CreateNode(knotposition, count);
                count++;
            }
            ViewNodes(false);
        }
    }
    
    /// <summary>
    /// Unter diesem GameObject werden die anderen Nodes gespeichert. Die Funktion ist nur da, um im UnityEditor eine bessere Übersicht zu haben.
    /// </summary>
    private void CreateParentNode()
    {
        parentNode = new GameObject("Nodes");
        parentNode.transform.SetParent(transform, false);
    }

    private GameObject CreateNode(Vector3 position, int index)
    {
        if (parentNode == null)
        {
            CreateParentNode();
        }
        var node = Instantiate(preNode, parentNode.transform, false);
        node.transform.localPosition = parentNode.transform.InverseTransformPoint(position);
        var scale = Convert.ToSingle(GetRadius() * 3);
        node.transform.localScale = new Vector3(scale, scale, scale);
        node.transform.hasChanged = false;
        node.GetComponent<ArtNode>().SetIndex(index);
        node.GetComponent<InteractableUnityEventWrapper>().WhenHover.AddListener(OnNodeHover);
        node.GetComponent<InteractableUnityEventWrapper>().WhenUnhover.AddListener(OnNodeUnhover);
        return node;
    }

    private GameObject apperiancePrefab;
    
    /// <summary>
    /// Erzeugt an jedem Node das jeweilige Objekt.
    /// </summary>
    /// <param name="prefab"> Objekt welches erzeugt wird. </param>
    public void AddInstantiateObject(GameObject prefab)
    {
        if (apperiancePrefab != null)
            if (apperiancePrefab == prefab)
            {
                splineInstantiate.Randomize();
                UpdateInstances();
                return;
            }
        RemoveInstantiateObjects();
        apperiancePrefab = prefab;
        AddInstantiateObjects();
        /*
        var items = splineInstantiate.itemsToInstantiate;
        var newItems = new SplineInstantiate.InstantiableItem [items.Length + 1];
        for (int i = 0; i < items.Length; i++)
            newItems[i] = items[i];
        newItems[newItems.Length] = prefab;
        splineInstantiate.itemsToInstantiate = newItems;*/
    }

    private void AddInstantiateObjects()
    {
        var items = SetRandomColorsOfInstantiableItems(apperiancePrefab);
        splineInstantiate.itemsToInstantiate = items;
        splineInstantiate.enabled = true;
        splineInstantiate.Randomize();
        RecallculateInstantiateObjects();
        UpdateInstances();
    }
    
    /// <summary>
    /// Entfernt die Objekte an den Nodes.
    /// </summary>
    public void RemoveInstantiateObjects()
    {
        if (splineInstantiate.itemsToInstantiate.Length < 1 || splineInstantiate == null)
            return;
        var items = splineInstantiate.itemsToInstantiate;
        foreach (var item in items)
            Destroy(item.Prefab);
        splineInstantiate.itemsToInstantiate = Array.Empty<SplineInstantiate.InstantiableItem>();
        splineInstantiate.enabled = false;
        apperiancePrefab = null;
    }

    private float baseScale = 0.03f;
    private float yScale = 0.06f;
    
    /// <summary>
    /// Verändert die Farbe Random von dem Objekt
    /// </summary>
    /// <param name="prefab"> Das Objekt, welches eine andere Farbe haben soll. </param>
    /// <returns></returns>
    private SplineInstantiate.InstantiableItem[] SetRandomColorsOfInstantiableItems(GameObject prefab)
    {
        if (splineInstantiate == null)
            splineInstantiate = gameObject.AddComponent<SplineInstantiate>();
        splineInstantiate.itemsToInstantiate ??= Array.Empty<SplineInstantiate.InstantiableItem>();
        var items = new SplineInstantiate.InstantiableItem[5];
        for (int i = 0; i < items.Length; i++)
        {
            var instance = Instantiate(prefab);
            if (instance.transform.localScale.x.Equals(instance.transform.localScale.y))
                instance.transform.localScale = new Vector3(baseScale, baseScale, baseScale);
            else
                instance.transform.localScale = new Vector3(baseScale, yScale, baseScale);
            SetColorOfItem(instance, i);
            items[i].Prefab = instance;
            items[i].Probability = 20f;
        }
        return items;
    }

    private Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta };

    private void SetColorOfItem(GameObject item, int index)
    {
        item.GetComponent<MeshRenderer>().material.color = colors[index];
    }

    private SplineInstantiate.InstantiableItem RecallculateInstantiateObject(SplineInstantiate.InstantiableItem item)
    {
        /*if (item.Prefab.transform.localScale.x <= 3000f)
            item.Prefab.transform.localScale = itemScale * ((float)Math.Log(GetRadius(), 1000) + 0.8f);
        else if (item.Prefab.transform.localScale.x is > 1f and <= 2f)
            item.Prefab.transform.localScale = itemScale * (GetRadius() * 3f);*/
        return item;
    }
    
    /// <summary>
    /// Updated die Objekte nach dem sie verändert wurden.
    /// </summary>
    public void RecallculateInstantiateObjects()
    {
        /*var items = splineInstantiate.itemsToInstantiate;
        for (int i = 0; i < items.Length; i++)
            items[i] = RecallculateInstantiateObject(items[i]);
        splineInstantiate.itemsToInstantiate = items;*/
        UpdatePositionOfInstances(splineInstantiate.itemsToInstantiate[0]);
        UpdateInstances();
    }

    private void UpdatePositionOfInstances(SplineInstantiate.InstantiableItem item)
    {
        if (item.Prefab.transform.localScale.x.Equals(item.Prefab.transform.localScale.y))
            splineInstantiate.MinPositionOffset = new Vector3(0, (GetRadius() * -1f) -baseScale + 0.022f, 0);
        else
            splineInstantiate.MinPositionOffset = new Vector3(0, (GetRadius() * -1f) -yScale + 0.038f, 0); // Es wird eine andere Skalierung verwendet, weil das eine Objekt ein Zylinder ist und das andere eine Kugel
        
        splineInstantiate.MaxPositionOffset = splineInstantiate.MinPositionOffset;
    }

    /// <summary>
    /// Macht Objekte sichtbar/unsichtbar.
    /// </summary>
    /// <param name="enable"> True sichtbar, False unsichtbar </param>
    private void EnablePrefabsOfInstantiableItems(bool enable)
    {
        var items = splineInstantiate.itemsToInstantiate;
        foreach (var item in items)
        {
            item.Prefab.gameObject.SetActive(enable);
        }
    }

    private void UpdateInstances()
    {
        EnablePrefabsOfInstantiableItems(true);
        splineInstantiate.UpdateInstances();
        EnablePrefabsOfInstantiableItems(false);
    }

    private float oldSpaceing;
    
    /// <summary>
    /// Stellt die Distanz zwischen den Node Objekten ein.
    /// </summary>
    /// <param name="value"> Abstand der Objekte. </param>
    public void SetSpacing(float value)
    {
        if (oldSpaceing <= value) // der Wert kann nicht eingestellt werden, wenn sich die Werte überschneiden. Also Max Wert darf nicht kleiner als Min Wert sein und andersherum.
        {
            splineInstantiate.MaxSpacing = value;
            splineInstantiate.MinSpacing = splineInstantiate.MaxSpacing;
        }
        else
        {
            splineInstantiate.MinSpacing = value;
            splineInstantiate.MaxSpacing = splineInstantiate.MinSpacing;
        }
        oldSpaceing = value;
        UpdateInstances();
    }

    public int GetInstanceCount() { return splineInstantiate.itemsToInstantiate.Length; }
    
    public float GetInstanceSpace() { return splineInstantiate.MinSpacing; }

    private void RemoveInstantiateObject(SplineInstantiate.InstantiableItem prefab)
    {
        /*
        var items = splineInstantiate.itemsToInstantiate;
        var newItems = new SplineInstantiate.InstantiableItem [items.Length - 1];
        var j = 0;
        for (int i = 0; i < items.Length; i++, j++)
        {
            if (items[i].Equals(prefab))
                i++;
            newItems[j] = items[i];
        }
        splineInstantiate.itemsToInstantiate = newItems;*/
    }
	
	public GameObject[] getNodes() { return nodes; }

    private void OnNodeHover() { handGrab.enabled = false; }

    private void OnNodeUnhover() { handGrab.enabled = true; }
    
    private void DeleteNodes()
    {
        if (parentNode != null)
        {
            Destroy(parentNode.gameObject);
            parentNode = null;
            nodes = null;
            return;
        }
        
        foreach (var node in nodes)
        {
            Destroy(node.gameObject);
        }
        parentNode = null;
        nodes = null;
    }

    /// <summary>
    /// Die Nodes werden in dieser Funktion sichtbar gemacht, falls die Nodes als Gameobject noch nicht existieren, werden sie erst erzeugt
    /// </summary>
    /// <param name="view"> True: sichtbar, False: unsichtbar </param>
    public void ViewNodes(bool view)
    {
        if (nodes == null || nodes.Length == 0)
            CreateNodes();
        if (nodes == null || nodes.Length == 0)
        {
            return;
        }
        foreach (var node in nodes)
        {
            node.gameObject.SetActive(view);
            node.transform.localScale = Vector3.one * Convert.ToSingle(GetRadius() * 3);
        }
    }

    /// <summary>
    /// Die Nodes im Spline werden aktualisiert.
    /// </summary>
    /// <param name="artNode"> Der Node welcher verändert wurde, z.B. weil er verschoben wurde </param>
    public void OnNodeChanged(ArtNode artNode)
    {
        var index = artNode.GetIndex();
        var bezierKnot = spline[index];
        bezierKnot.Position = artNode.transform.localPosition;
        spline[index] = bezierKnot;
        if (splineInstantiate.itemsToInstantiate.Length > 0)
            UpdateInstances();
    }

    /// <summary>
    /// Aktivert, dass man eigene Nodes erstellt und diese zum Spline hinzugefügt werden.
    /// </summary>
    /// <param name="enable"> True: Nodes erstellen aktiviert, False: deaktiviert </param>
    public void NodePlacementMode(bool enable)
    {
        enableNodePlacement = enable;
        if (enable)
        {
            SetRandomColor();
            meshRenderer.enabled = false;
        }
        else
        {
            ViewNodes(false);
        }
    }

    private void PlaceNode(InputAction.CallbackContext context)
    {
        PlaceNode(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch ));
    }

    private GameObject node;

    public void PlaceNode(GameObject hand)
    {
        PlaceNode(hand.transform.position);
    }

    /// <summary>
    /// Plaziert einen Node und fügt ihn dem Spline hinzu.
    /// </summary>
    /// <param name="position"> Position des Nodes. </param>
    public void PlaceNode(Vector3 position)
    {
        if (!enableNodePlacement)
            return;
        spline.Add(transform.InverseTransformPoint(position));
        if (spline.ToArray().Length < 2)
            meshRenderer.enabled = false;
        else
            meshRenderer.enabled = true;
        node = CreateNode(position, nodes.Length);
        if (nodes.Length < spline.ToArray().Length)
        {
            var oldNodes = nodes;
            nodes = new GameObject[spline.ToArray().Length];
            for (int i = 0; i < oldNodes.Length; i++)
                nodes[i] = oldNodes[i];
        }
        nodes[spline.ToArray().Length - 1] = node;
        if (splineInstantiate.itemsToInstantiate.Length > 0)
            RecallculateInstantiateObjects();
    }
    
    /// <summary>
    /// Ersetzt das Material des Splines.
    /// </summary>
    /// <param name="newMaterial"> Neues Material </param>
    public void SetMaterial(Material newMaterial)
    {
        var materials = meshRenderer.materials;
        materials[1] = newMaterial;
        meshRenderer.materials = materials;
    }

    public Material GetMaterial() { return meshRenderer.materials[1]; }

    private Color _oldColor;

    public void SetRandomColor()
    {
        var materials = meshRenderer.materials;
        materials[1].color = Random.ColorHSV();
        _oldColor = materials[1].color;
        meshRenderer.materials = materials;
    }

    public void SetColor(Color color)
    {
        var materials = meshRenderer.materials;
        materials[1].color = color;
        _oldColor = materials[1].color;
        meshRenderer.materials = materials;
    }

    /// <summary>
    /// Aktiviert die Outline des Splines.
    /// </summary>
    /// <param name="enable"> True: Outline aktiviert, False: Outline deaktiviert </param>
    public void EnableOutline(bool enable)
    {
        var materials = meshRenderer.materials;
        if (enable)
            materials[0] = outlineMaterial;
        else
            materials[0] = nullMaterial; // es wird ein nullMaterial hinzugefügt, statt das Material einfach zu löschen, weil Unity das nicht mag, wenn Materialien zur laufzeit gelöscht werden. Das Material hat aber keine Visuale veränderung des Splines.
        meshRenderer.materials = materials;
    }

    public Color GetColor() { return meshRenderer.materials[1].color; }

    private void SetRandomSplineNodes()
    {
        for (int i = 0; i < Random.Range(3, 15); i++)
            spline.Add(new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 2f), Random.Range(-1f, 1f)));
    }

    public void SetNodes(Vector3[] wurmNodes)
    {
        spline.Clear();
        foreach (var node in wurmNodes)
        {
            spline.Add(node);
        }
    }

    public Vector3[] GetNodes()
    {
        var wurmNodes = new Vector3[spline.Count];
        for (int i = 0; i < spline.Count; i++)
        {
            wurmNodes[i] = spline.ToArray()[i].Position;
        }
        return wurmNodes;
    }

    public void SetRandomRadius()
    {
        splineExtrude.Radius = Random.Range(0.01f, 0.1f);
        if (splineInstantiate.itemsToInstantiate.Length > 0)
        {
            RecallculateInstantiateObjects();
        }
    }

    public void SetRadius(float radius)
    {
        splineExtrude.Radius = radius;
        if (splineInstantiate.itemsToInstantiate.Length > 0)
        {
            RecallculateInstantiateObjects();
        }
    }

    public float GetRadius() { return splineExtrude.Radius; }
}