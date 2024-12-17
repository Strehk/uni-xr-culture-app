using System;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Wurm : MonoBehaviour
{
    [SerializeField] public InputActionAsset controls;
    [HideInInspector] [SerializeField] private Spline spline;
    [HideInInspector] [SerializeField] private SplineContainer splineContainer;
    [HideInInspector] [SerializeField] private SplineExtrude splineExtrude;

    [HideInInspector] [SerializeField] private MeshRenderer meshRenderer;
    [HideInInspector] [SerializeField] private MeshCollider meshCollider;
    [HideInInspector] [SerializeField] private bool selected;
    
    [HideInInspector] [SerializeField] private bool enableNodePlacement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Setup();
        var debugActionMap = controls.FindActionMap("Debug");
        var generateInputAction = debugActionMap.FindAction("Generate");
        generateInputAction.performed += Generate;

        var playerActionMap = controls.FindActionMap("Player");
        var moveObjectInputAction = playerActionMap.FindAction("MoveObject");
        moveObjectInputAction.performed += MoveObject;

        var selectObjectInputAction = playerActionMap.FindAction("SelectObject");
        selectObjectInputAction.performed += SelectObject;
        
        var nodeCreationAction = debugActionMap.FindAction("EnableNodeCreation");
        nodeCreationAction.performed += NodePlacementMode;
        
        var nodePlacement = debugActionMap.FindAction("NodePlacement");
        nodePlacement.performed += PlaceNode;
    }

    private void OnMouseOver()
    {
        SetRandomColor();
    }

    private void Setup()
    {
        gameObject.SetActive(false);
        splineContainer = gameObject.AddComponent<SplineContainer>();
        spline = GetComponent<SplineContainer>().Spline;
        splineExtrude = gameObject.AddComponent<SplineExtrude>();
        splineExtrude.Container = splineContainer;
        splineExtrude.RebuildOnSplineChange = true;
        splineExtrude.SegmentsPerUnit = 20;
        gameObject.GetComponent<MeshFilter>().mesh = new Mesh();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        ColliderSurface surface = gameObject.AddComponent<ColliderSurface>();
        surface.InjectCollider(meshCollider);
        RayInteractable rayInteractable = gameObject.AddComponent<RayInteractable>();
        rayInteractable.InjectAllRayInteractable(surface);
    }

    private void Generate(InputAction.CallbackContext context)
    {
        gameObject.SetActive(true);
        spline.Clear();
        SetRandomSplineNodes();
        SetRandomRadius();
        //SetRandomPosition();
        SetRandomColor();
    }
    
    public void OnButtonClick()
    {
        Generate(default);
    }
    
    private void MoveObject(InputAction.CallbackContext context)
    {
        if (!selected) return;
        var moveObjectInput = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
        transform.position += moveObjectInput * 0.01f;
    }

    private void SelectObject(InputAction.CallbackContext context)
    {
        selected = !selected;
    }

    private void SetMaterial(Material newMaterial)
    {
        meshRenderer.material = newMaterial;
    }

    private void SetRandomColor()
    {
        SetMaterial(new Material(Shader.Find(new String("Universal Render Pipeline/Lit")))
        {
            color = Random.ColorHSV()
        });
    }

    private void NodePlacementMode(InputAction.CallbackContext context)
    {
        if (enableNodePlacement)
            enableNodePlacement = false;
        else
        {
            enableNodePlacement = true;
            gameObject.SetActive(false);
            spline.Clear();
        }
    }

    private void PlaceNode(InputAction.CallbackContext context)
    {
        if (enableNodePlacement)
            spline.Add(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch));
    }

    public void SetSplineNodes(Vector3[] nodes)
    {
        foreach (var node in nodes)
        {
            spline.Add(node);
        }
    }

    private void SetRandomSplineNodes()
    {
        for (int i = 0; i < Random.Range(3, 15); i++)
            spline.Add(new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 2f), Random.Range(-1f, 1f)));
    }

    private void SetRandomRadius()
    {
        splineExtrude.Radius = Random.Range(0.01f, 0.1f);
    }


    // Update is called once per frame
    void Update()
    {
    }
}