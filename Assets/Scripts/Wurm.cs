# if UNITY_EDITOR
using System;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Surfaces;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
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
        
        splineInstantiate = gameObject.GetComponent<SplineInstantiate>();
        if (splineInstantiate != null)
        {
            splineInstantiate.Container = splineContainer;
            splineInstantiate.enabled = false;
        }
        
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

    public void CreateNodes()
    {
        if (parentNode == null)
        {
            parentNode = new GameObject("Nodes");
            parentNode.transform.SetParent(transform, false);
        }

        if (gameObject.GetComponentInChildren<ArtNode>() == null && (nodes == null || nodes.Length == 0))
        {
            var count = 0;
            nodes = new GameObject[spline.Count];
            foreach (var knot in spline.ToArray())
            {
                Debug.Log(count + " Knoten: " + knot);
                var node = Instantiate(preNode, parentNode.transform, false);
                node.transform.localPosition = knot.Position;
                var scale = Convert.ToSingle(GetRadius() * 3);
                node.transform.localScale = new Vector3(scale, scale, scale);
                node.transform.hasChanged = false;
                node.GetComponent<ArtNode>().SetIndex(count);
                node.GetComponent<InteractableUnityEventWrapper>().WhenHover.AddListener(OnNodeHover);
                node.GetComponent<InteractableUnityEventWrapper>().WhenUnhover.AddListener(OnNodeUnhover);
                nodes[count] = node;
                count++;
            }
            ViewNodes(false);
        }
    }

    public void AddInstantiateObject(GameObject prefab)
    {
        RemoveInstantiateObjects();
        var items = SetRandomColorsOfInstantiableItem(prefab);
        splineInstantiate.itemsToInstantiate = items;
        RecallculateInstantiateObjects();
        splineInstantiate.enabled = true;
        EnablePrefabsOfInstantiableItems(false);
        /*
        var items = splineInstantiate.itemsToInstantiate;
        var newItems = new SplineInstantiate.InstantiableItem [items.Length + 1];
        for (int i = 0; i < items.Length; i++)
            newItems[i] = items[i];
        newItems[newItems.Length] = prefab;
        splineInstantiate.itemsToInstantiate = newItems;*/
    }
    
    public void RemoveInstantiateObjects()
    {
        var items = splineInstantiate.itemsToInstantiate;
        foreach (var item in items)
            Destroy(item.Prefab);
        splineInstantiate.itemsToInstantiate = Array.Empty<SplineInstantiate.InstantiableItem>();
        splineInstantiate.enabled = false;
    }

    private SplineInstantiate.InstantiableItem[] SetRandomColorsOfInstantiableItem(GameObject prefab)
    {
        var items = new SplineInstantiate.InstantiableItem[10];
        for (int i = 0; i < items.Length; i++)
        {
            var instance = Instantiate(prefab);
            instance.GetComponent<MeshRenderer>().material.color = Random.ColorHSV(0.5f, 1f, 0.5f, 1f, 0.8f, 1f);
            items[i].Prefab = instance;
            items[i].Probability = 10f;
        }
        return items;
    }

    private SplineInstantiate.InstantiableItem RecallculateInstantiateObject(SplineInstantiate.InstantiableItem item)
    {
        item.Prefab.transform.localScale *= GetRadius() * 4f;
        splineInstantiate.MinPositionOffset = new Vector3(0, -2f * GetRadius(), 0);
        splineInstantiate.MaxPositionOffset = splineInstantiate.MinPositionOffset;
        return item;
    }
    
    public void RecallculateInstantiateObjects()
    {
        var items = splineInstantiate.itemsToInstantiate;
        for (int i = 0; i < items.Length; i++)
            items[i] = RecallculateInstantiateObject(items[i]);
        
    }

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

    public void ViewNodes(bool view)
    {
        if (nodes == null || nodes.Length == 0)
            CreateNodes();
        if (nodes == null || nodes.Length == 0)
        {
            Debug.LogError("ViewNodes: nodes is null");
            return;
        }
        foreach (var node in nodes)
        {
            node.gameObject.SetActive(view);
            node.transform.localScale = Vector3.one * Convert.ToSingle(GetRadius() * 3);
        }
    }

    public void OnNodeChanged(ArtNode artNode)
    {
        var index = artNode.GetIndex();
        var bezierKnot = spline[index];
        bezierKnot.Position = artNode.transform.localPosition;
        spline[index] = bezierKnot;
        if (splineInstantiate.itemsToInstantiate.Length > 0)
            UpdateInstances();
    }

    public void NodePlacementMode(bool enable)
    {
       
        enableNodePlacement = enable;
    }

    private void PlaceNode(InputAction.CallbackContext context)
    {
        if (enableNodePlacement)
            spline.Add(transform.InverseTransformPoint( OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch )));
    }
    
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

    public void EnableOutline(bool enable)
    {
        var materials = meshRenderer.materials;
        if (enable)
            materials[0] = outlineMaterial;
        else
            materials[0] = nullMaterial;
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
            RecallculateInstantiateObjects();
    }

    public void SetRadius(float radius)
    {
        splineExtrude.Radius = radius;
        if (splineInstantiate.itemsToInstantiate.Length > 0)
            RecallculateInstantiateObjects();
    }

    public float GetRadius() { return splineExtrude.Radius; }
}
# endif