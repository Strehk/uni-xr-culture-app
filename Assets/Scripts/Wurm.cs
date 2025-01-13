using System;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Surfaces;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Wurm : MonoBehaviour
{
    [SerializeField] private GameObject prenode;
    [SerializeField] public InputActionAsset controls;
    
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

    private void Awake()
    {
        Setup();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        var debugActionMap = controls.FindActionMap("Debug");
        
        var playerActionMap = controls.FindActionMap("Player");

        var selectObjectInputAction = playerActionMap.FindAction("SelectObject");
        selectObjectInputAction.performed += SelectObject;
        
        var nodePlacement = debugActionMap.FindAction("NodePlacement");
        nodePlacement.performed += PlaceNode;
    }

    private void Setup()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        
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

    private void Generate(InputAction.CallbackContext context)
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

    private Material oldMaterial;

    public void OnHoverEnter()
    {
        SetMaterial(new Material(Shader.Find(new String("Universal Render Pipeline/Lit")))
        {
            color = Color.blue
        });
        
    }

    public void OnHoverExit()
    {
        SetMaterial(oldMaterial);
    }

    public void OnSelect()
    {
        SetMaterial(new Material(Shader.Find(new String("Universal Render Pipeline/Lit")))
        {
            color = Color.red
        });
    }

    public void OnUnselect()
    {
        Debug.Log("Unselect");
    }
    
    public void OnButtonClick()
    {
        Generate(default);
    }

    private void CreateNodes()
    {
        if (parentNode == null)
        {
            parentNode = new GameObject("Nodes");
            parentNode.transform.SetParent(transform, false);
        }

        if (gameObject.GetComponentInChildren<ArtNode>() == null && nodes == null)
        {
            var count = 0;
            nodes = new GameObject[spline.Count];
            foreach (var knot in spline.ToArray())
            {
                Debug.Log(count + " Knoten: " + knot);
                var node = Instantiate(prenode, parentNode.transform, false);
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
	
	
	public GameObject[] getNodes()
	{
		return nodes;
	}

    private void OnNodeHover()
    {
        handGrab.enabled = false;
    }

    private void OnNodeUnhover()
    {
        handGrab.enabled = true;
    }
    
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
        if (view && nodes == null)
            CreateNodes();
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
    }
   
    private void MoveObject(InputAction.CallbackContext context)
    {
        if (!selected) return;
        var moveObjectInput = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
        transform.position += moveObjectInput * 0.01f;
    }

    private void SelectObject(InputAction.CallbackContext context)
    {
        splineInstantiate.enabled = true;
        selected = !selected;
    }

    public void NodePlacementMode(bool enable)
    {
        if (enable)
        {
            enableNodePlacement = true;
            spline.Clear();
            DeleteNodes();
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }
        else
        {
            enableNodePlacement = false;
        }
    }

    private void PlaceNode(InputAction.CallbackContext context)
    {
        if (enableNodePlacement)
            spline.Add(transform.InverseTransformPoint( OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch )));
    }
    
    private void SetMaterial(Material newMaterial)
    {
        meshRenderer.material = newMaterial;
    }

    public Material GetMaterial()
    {
        return meshRenderer.material;
    }

    private void SetRandomColor()
    {
        
        meshRenderer.material.color = Random.ColorHSV();
        oldMaterial = meshRenderer.material;
    }

    public void SetColor(Color color)
    {
        meshRenderer.material.color = color;
        oldMaterial = meshRenderer.material;
    }

    public Color GetColor()
    {
        return meshRenderer.material.color;
    }

    private void SetRandomSplineNodes()
    {
        for (int i = 0; i < Random.Range(3, 15); i++)
            spline.Add(new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 2f), Random.Range(-1f, 1f)));
    }

    public void SetNodes(Vector3[] nodes)
    {
        spline.Clear();
        foreach (var node in nodes)
        {
            spline.Add(node);
        }
        

    }

    public Vector3[] GetNodes()
    {
        var nodes = new Vector3[spline.Count];
        for (int i = 0; i < spline.Count; i++)
        {
            nodes[i] = spline.ToArray()[i].Position;
        }
        return nodes;
    }

    private void SetRandomRadius()
    {
        splineExtrude.Radius = Random.Range(0.01f, 0.1f);
    }

    public void SetRadius(float radius)
    {
        splineExtrude.Radius = radius;
    }

    public float GetRadius()
    {
        return splineExtrude.Radius;
    }


    // Update is called once per frame
    void Update()
    {
    }

    
}