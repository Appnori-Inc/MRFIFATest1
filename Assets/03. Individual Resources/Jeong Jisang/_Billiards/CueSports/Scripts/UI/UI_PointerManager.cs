using System.Collections;
using System.Collections.Generic;
using NetworkManagement;
using System;

namespace Billiards
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Linq;

    [System.Obsolete]
    [Serializable]
    public class GraphicRaycasterDict
    {
        public List<Canvas> Keylist;
        public Dictionary<Canvas, GraphicRaycaster> Dict;

        public GraphicRaycasterDict()
        {
            Keylist = new List<Canvas>();
            Dict = new Dictionary<Canvas, GraphicRaycaster>();
        }

        public GraphicRaycaster this[int index]
        {
            get => Dict[Keylist[index]];
            set => Dict[Keylist[index]] = value;
        }
    }


    [System.Obsolete("NotUse. use in PICO", true)]
    public class UI_PointerManager : MonoBehaviour
    {
        public static UI_PointerManager instance;

        public enum PointerState
        {
            All, Swap, Hide
        }

        public PointerState state = PointerState.All;

        private PointerState current_state = PointerState.All;

        public GameObject[] poses = new GameObject[1];
        public Camera camera;

        public List<Canvas> ui_canvas
        {
            get => grs.Keylist;
            set => grs.Keylist = value;
        }

        public Dictionary<Canvas, GraphicRaycaster> graphicRaycasterDict
        {
            get => grs.Dict;
            set => grs.Dict = value;
        }

        public GraphicRaycasterDict grs;
        private PointerEventData ped;

        private PointerData[] pointerDatas;

        public Color color = Color.white;
        public float thickness = 0.002f;
        public Color clickColor = Color.green;
        public Shader shader_ball;//"Lightweight Render Pipeline/Lit"


        public float dragThreshold = 0.001f;
        public bool isMainCollder = false;
        private int current_cotroller_num = 0;

        public GameObject selectContact = null;

        // Start is called before the first frame update
        void Awake()
        {
            //if (instance == null)
            //    instance = this;
            //if (instance != this)
            //    Destroy(this.gameObject);

            //grs = new GraphicRaycasterDict();
            //BilliardsDataContainer.Instance.LeftEyeCamera.OnDataChanged += SetCamera;
        }

        void SetCamera(Camera cam)
        {
            //if (camera == null && cam != null)
            //{
            //    camera = cam;
            //}

            //BilliardsDataContainer.Instance.LeftEyeCamera.OnDataChanged -= SetCamera;
        }

        void Start()
        {
            //shader_ball = Shader.Find("Lightweight Render Pipeline/Lit");
            //if (poses.Length == 0)
            //{
            //    Debug.LogError("No SteamVR_Behaviour_Pose component found on this object");
            //    return;
            //}

            //pointerDatas = new PointerData[poses.Length];

            //for (int i = 0; i < pointerDatas.Length; i++)
            //{
            //    pointerDatas[i] = new PointerData();
            //    //Debug.Log(pointerDatas[i].pose);
            //    pointerDatas[i].pose = poses[i];

            //    GameObject holder = new GameObject("UIPointer");
            //    holder.transform.parent = poses[i].transform;
            //    holder.transform.localPosition = Vector3.zero;
            //    holder.transform.localRotation = Quaternion.identity;
            //    // holder.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);            
            //    holder.transform.localScale = Vector3.one * (1f / transform.lossyScale.x);

            //    thickness *= transform.lossyScale.x;

            //    pointerDatas[i].pointer_line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    pointerDatas[i].pointer_line.transform.parent = holder.transform;
            //    pointerDatas[i].pointer_line.layer = LayerMask.NameToLayer("UI");
            //    pointerDatas[i].pointer_line.transform.localScale = new Vector3(thickness, thickness, 100f);
            //    pointerDatas[i].pointer_line.transform.localPosition = new Vector3(0f, 0f, 50f);
            //    pointerDatas[i].pointer_line.transform.localRotation = Quaternion.identity;
            //    Destroy(pointerDatas[i].pointer_line.GetComponent<Collider>());

            //    Material mat_line = new Material(Shader.Find("Unlit/Color"));
            //    mat_line.SetColor("_Color", color);
            //    pointerDatas[i].pointer_line.GetComponent<MeshRenderer>().material = mat_line;

            //    pointerDatas[i].pointer_ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //    pointerDatas[i].pointer_ball.transform.parent = holder.transform;
            //    pointerDatas[i].pointer_ball.transform.localScale = Vector3.one * 0.1f * transform.lossyScale.x;
            //    Destroy(pointerDatas[i].pointer_ball.GetComponent<Collider>());
            //    Destroy(pointerDatas[i].pointer_ball.GetComponent<MeshRenderer>());

            //    //Material mat_ball = new Material(shader_ball == null ? Shader.Find("Standard") : shader_ball);
            //    //mat_ball.SetColor("_Color", Color.yellow);
            //    //mat_ball.EnableKeyword("_EMISSION");
            //    //mat_ball.SetColor("_EmissionColor", Color.red);
            //    //pointerDatas[i].pointer_ball.GetComponent<Renderer>().material = mat_ball;
            //}

            //if (EventSystem.current != null)
            //{
            //    EventSystem.current.gameObject.SetActive(false);
            //}

            //ped = new PointerEventData(null);
            ////grs = new GraphicRaycaster[ui_canvas.Count];

            //using (var e = ui_canvas.GetEnumerator())
            //{
            //    while (e.MoveNext())
            //    {
            //        if (!e.Current.TryGetComponent<GraphicRaycaster>(out var component))
            //        {
            //            component = e.Current.gameObject.AddComponent<GraphicRaycaster>();
            //        }

            //        graphicRaycasterDict[e.Current] = component;

            //        if (isMainCollder)
            //        {
            //            BoxCollider boxCollider = e.Current.gameObject.AddComponent<BoxCollider>();
            //            RectTransform rectTransform = e.Current.GetComponent<RectTransform>();

            //            boxCollider.isTrigger = true;
            //            boxCollider.size = new Vector3(rectTransform.rect.width, rectTransform.rect.height, 0.01f / rectTransform.localScale.z);
            //        }

            //        e.Current.gameObject.layer = LayerMask.NameToLayer("UI");
            //    }
            //}

            ////for (int i = 0; i < ui_canvas.Count; i++)
            ////{
            ////    grs[i] = ui_canvas[i].GetComponent<GraphicRaycaster>();
            ////    if (grs[i] == null)
            ////    {
            ////        grs[i] = ui_canvas[i].gameObject.AddComponent<GraphicRaycaster>();
            ////    }

            ////    if (isMainCollder)
            ////    {
            ////        BoxCollider boxCollider = ui_canvas[i].gameObject.AddComponent<BoxCollider>();
            ////        RectTransform rectTransform = ui_canvas[i].GetComponent<RectTransform>();

            ////        boxCollider.isTrigger = true;
            ////        boxCollider.size = new Vector3(rectTransform.rect.width, rectTransform.rect.height, 0.01f / rectTransform.localScale.z);
            ////    }

            ////    ui_canvas[i].gameObject.layer = LayerMask.NameToLayer("UI");
            ////}

            //onInitialized?.Invoke(this);
            //onInitialized = null;
        }

        private static event Action<UI_PointerManager> onInitialized;
        public static event Action<UI_PointerManager> OnInitialized
        {
            add
            {
                if (instance != null)
                {
                    value?.Invoke(instance);
                }
                else
                {
                    onInitialized += value;
                }
            }

            remove
            {
                onInitialized -= value;
            }
        }


        public void AddCanvas(Canvas canvas)
        {
            //if (ui_canvas.Contains(canvas))
            //    return;

            //ui_canvas.Add(canvas);
            //if (!canvas.TryGetComponent<GraphicRaycaster>(out var raycaster))
            //{
            //    raycaster = canvas.gameObject.AddComponent<GraphicRaycaster>();
            //}

            //graphicRaycasterDict[canvas] = raycaster;

            //canvas.gameObject.layer = LayerMask.NameToLayer("UI");
        }


        private void Update()
        {
            //CheckChangeState();

            //if (current_state == PointerState.Hide)
            //{
            //    return;
            //}

            //CheckUIPointer();
        }

        private void CheckChangeState()
        {
            //if (current_state != state)
            //{
            //    switch (state)
            //    {
            //        case PointerState.All:
            //        {
            //            for (int i = 0; i < pointerDatas.Length; i++)
            //            {
            //                pointerDatas[i].pose.transform.Find("UIPointer").gameObject.SetActive(true);
            //            }
            //        }
            //        break;
            //        case PointerState.Swap:
            //        {
            //            for (int i = 0; i < pointerDatas.Length; i++)
            //            {
            //                if (current_cotroller_num == i)
            //                {
            //                    pointerDatas[i].pose.transform.Find("UIPointer").gameObject.SetActive(true);
            //                }
            //                else
            //                {
            //                    pointerDatas[i].pose.transform.Find("UIPointer").gameObject.SetActive(false);
            //                }
            //            }
            //        }
            //        break;
            //        case PointerState.Hide:
            //        {
            //            for (int i = 0; i < pointerDatas.Length; i++)
            //            {
            //                pointerDatas[i].pose.transform.Find("UIPointer").gameObject.SetActive(false);
            //            }
            //        }
            //        break;
            //    }
            //    for (int i = 0; i < pointerDatas.Length; i++)
            //    {
            //        pointerDatas[i].isPress = false;
            //        pointerDatas[i].isSelectOut = false;

            //        if (pointerDatas[i].previousContact != null)
            //        {
            //            PointerExit(pointerDatas[i].previousContact);
            //        }

            //        if (pointerDatas[i].pushContact != null)
            //        {
            //            PointerUp(pointerDatas[i].pushContact);
            //        }

            //        pointerDatas[i].previousContact = null;
            //        pointerDatas[i].pushContact = null;
            //    }

            //    current_state = state;
            //}
            //else if (current_state == PointerState.Swap)
            //{
            //    for (int i = 0; i < pointerDatas.Length; i++)
            //    {
            //        if (Controller.UPvr_GetKeyDown(0, Pvr_KeyCode.TRIGGER) || Input.GetKeyDown(KeyCode.Mouse0))
            //        {
            //            if (i != current_cotroller_num)
            //            {
            //                current_cotroller_num = i;
            //                for (int j = 0; j < pointerDatas.Length; j++)
            //                {
            //                    if (current_cotroller_num == j)
            //                    {
            //                        pointerDatas[j].pose.transform.Find("UIPointer").gameObject.SetActive(true);
            //                    }
            //                    else
            //                    {
            //                        pointerDatas[j].pose.transform.Find("UIPointer").gameObject.SetActive(false);
            //                    }

            //                    pointerDatas[j].isPress = false;
            //                    pointerDatas[j].isSelectOut = false;

            //                    if (pointerDatas[j].previousContact != null)
            //                    {
            //                        PointerExit(pointerDatas[j].previousContact);
            //                    }

            //                    if (pointerDatas[j].pushContact != null)
            //                    {
            //                        PointerUp(pointerDatas[j].pushContact);
            //                    }

            //                    pointerDatas[j].previousContact = null;
            //                    pointerDatas[j].pushContact = null;
            //                }
            //                break;
            //            }
            //        }
            //    }
            //}
        }

        private void CheckUIPointer()
        {
            //for (int i = 0; i < pointerDatas.Length; i++)
            //{
            //    if (current_state == PointerState.Swap && i != current_cotroller_num)
            //    {
            //        continue;
            //    }

            //    Transform poseTr = pointerDatas[i].pose.transform;

            //    Ray ray = new Ray(poseTr.position, poseTr.forward);
            //    //int layer = 1 << LayerMask.NameToLayer("Player_Mine") | 1 << LayerMask.NameToLayer("RayFireHand");
            //    Debug.DrawRay(poseTr.position, poseTr.forward);
            //    RaycastHit hit;
            //    //Debug.Log("CheckUIPointer()");
            //    if (Physics.Raycast(ray, out hit, 50f))
            //    {
            //        pointerDatas[i].pointer_ball.gameObject.SetActive(true);
            //        pointerDatas[i].pointer_ball.transform.position = hit.point;

            //        if (PicoControll.GetTotalTrigger || Input.GetKey(KeyCode.Mouse0))
            //        {
            //            pointerDatas[i].pointer_line.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, hit.distance);
            //            pointerDatas[i].pointer_line.transform.GetComponent<MeshRenderer>().material.color = clickColor;
            //        }
            //        else
            //        {
            //            pointerDatas[i].pointer_line.transform.transform.localScale = new Vector3(thickness, thickness, hit.distance);
            //            pointerDatas[i].pointer_line.transform.GetComponent<MeshRenderer>().material.color = color;
            //        }
            //        pointerDatas[i].pointer_line.transform.localPosition = new Vector3(0f, 0f, hit.distance * 0.5f);

            //        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("UI"))
            //        {
            //            for (int j = 0; j < ui_canvas.Count; j++)
            //            {
            //                Canvas canvas = hit.transform.GetComponentInParent<Canvas>();

            //                if (canvas != null && ui_canvas[j].gameObject == canvas.gameObject)
            //                {
            //                    //
            //                    SetEvent(j, i, hit.point);
            //                    break;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (pointerDatas[i].isPress)
            //            {
            //                SetEvent(pointerDatas[i].index_lastCanvas, i, pointerDatas[i].pointer_ball.transform.position);
            //            }
            //            else
            //            {
            //                if (pointerDatas[i].previousContact != null)
            //                {
            //                    bool isSamePointer = false;

            //                    for (int j = 0; j < pointerDatas.Length; j++)
            //                    {
            //                        if (j == i)
            //                        {
            //                            continue;
            //                        }

            //                        if (pointerDatas[i].previousContact == pointerDatas[j].previousContact)
            //                        {
            //                            isSamePointer = true;
            //                        }
            //                    }

            //                    if (!isSamePointer)
            //                    {
            //                        PointerExit(pointerDatas[i].previousContact);
            //                    }

            //                    pointerDatas[i].previousContact = null;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (PicoControll.GetTotalTrigger || Input.GetKey(KeyCode.Mouse0))
            //        {
            //            pointerDatas[i].pointer_line.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, 50f);
            //            pointerDatas[i].pointer_line.transform.GetComponent<MeshRenderer>().material.color = clickColor;
            //        }
            //        else
            //        {
            //            pointerDatas[i].pointer_line.transform.transform.localScale = new Vector3(thickness, thickness, 50f);
            //            pointerDatas[i].pointer_line.transform.GetComponent<MeshRenderer>().material.color = color;
            //        }

            //        pointerDatas[i].pointer_line.transform.localPosition = new Vector3(0f, 0f, 25f);
            //        pointerDatas[i].pointer_ball.gameObject.SetActive(false);

            //        if (pointerDatas[i].isPress)
            //        {
            //            SetEvent(pointerDatas[i].index_lastCanvas, i, pointerDatas[i].pointer_ball.transform.position);
            //        }
            //        else
            //        {
            //            if (pointerDatas[i].previousContact != null)
            //            {
            //                bool isSamePointer = false;

            //                for (int j = 0; j < pointerDatas.Length; j++)
            //                {
            //                    if (j == i)
            //                    {
            //                        continue;
            //                    }

            //                    if (pointerDatas[i].previousContact == pointerDatas[j].previousContact)
            //                    {
            //                        isSamePointer = true;
            //                    }
            //                }

            //                if (!isSamePointer)
            //                {
            //                    PointerExit(pointerDatas[i].previousContact);
            //                }

            //                pointerDatas[i].previousContact = null;
            //            }
            //        }
            //    }
            //}
        }

        public void SetEvent(int index_canvas, int index_pointer, Vector3 endPos)
        {
            //pointerDatas[index_pointer].index_lastCanvas = index_canvas;
            //if (camera == null)
            //{
            //    Debug.LogError("UI_PointerCamera is null");
            //    return;
            //}
            //camera.tag = "MainCamera";
            //ped.position = camera.WorldToScreenPoint(endPos);
            //List<RaycastResult> results = new List<RaycastResult>();
            //grs[index_canvas].Raycast(ped, results);

            //bool isSamePointer = false;

            //for (int i = 0; i < pointerDatas.Length; i++)
            //{
            //    if (i == index_pointer)
            //    {
            //        continue;
            //    }

            //    if (pointerDatas[i].previousContact != null && pointerDatas[index_pointer].previousContact == pointerDatas[i].previousContact)
            //    {
            //        isSamePointer = true;
            //    }
            //}

            //if (results.Count != 0 && results[0].gameObject != null) // 
            //{

            //    if (pointerDatas[index_pointer].previousContact != null && results[0].gameObject != pointerDatas[index_pointer].previousContact)
            //    {
            //        if (!isSamePointer)
            //        {
            //            PointerExit(pointerDatas[index_pointer].previousContact);
            //        }
            //    }

            //    GameObject target = results[0].gameObject;
            //    if (target == selectContact)
            //    {
            //        pointerDatas[index_pointer].previousContact = results[0].gameObject;
            //        return;
            //    }

            //    while (target.transform.parent != null)
            //    {
            //        target = target.transform.parent.gameObject;
            //        if (target == selectContact)
            //        {
            //            pointerDatas[index_pointer].previousContact = results[0].gameObject;
            //            return;
            //        }
            //    }
            //}

            //ped.pointerCurrentRaycast = ((results.Count != 0) ? results[0] : new RaycastResult());

            //if (PicoControll.GetTotalTriggerDown || Input.GetKeyDown(KeyCode.Mouse0))
            //{
            //    ped.pointerPressRaycast = ((results.Count != 0) ? results[0] : new RaycastResult());

            //    //pointerDatas[index_pointer].pressStartPos = endPos - transform.position;
            //    pointerDatas[index_pointer].pressStartDir = pointerDatas[index_pointer].pointer_line.transform.forward;

            //    pointerDatas[index_pointer].isSelectOut = false;
            //    pointerDatas[index_pointer].isPress = true;

            //    if (results.Count != 0)
            //    {
            //        if (!isSamePointer && pointerDatas[index_pointer].previousContact != null && results[0].gameObject != pointerDatas[index_pointer].previousContact)
            //        {
            //            PointerExit(pointerDatas[index_pointer].previousContact);
            //        }

            //        pointerDatas[index_pointer].pushContact = results[0].gameObject;
            //        pointerDatas[index_pointer].previousContact = results[0].gameObject;
            //        PointerDown(results[0].gameObject, ped);
            //        pointerDatas[index_pointer].dragContact = GetDrag(results[0].gameObject, ped);
            //        pointerDatas[index_pointer].isDrag = false;
            //    }
            //    else
            //    {
            //        if (!isSamePointer && pointerDatas[index_pointer].previousContact != null)
            //        {
            //            PointerExit(pointerDatas[index_pointer].previousContact);
            //        }

            //        pointerDatas[index_pointer].pushContact = null;
            //        pointerDatas[index_pointer].previousContact = null;
            //        pointerDatas[index_pointer].dragContact = null;
            //        pointerDatas[index_pointer].isDrag = false;
            //    }
            //}
            //else if (PicoControll.GetTotalTriggerUp || Input.GetKeyUp(KeyCode.Mouse0))
            //{
            //    ped.pointerPressRaycast = ((results.Count != 0) ? results[0] : new RaycastResult());
            //    if (pointerDatas[index_pointer].dragContact != null && pointerDatas[index_pointer].isDrag)
            //    {
            //        Drag(pointerDatas[index_pointer].dragContact, ped);
            //        EndDrag(pointerDatas[index_pointer].dragContact, ped);

            //        if (pointerDatas[index_pointer].pushContact != null)
            //        {
            //            PointerUp(pointerDatas[index_pointer].pushContact);
            //        }
            //    }
            //    else
            //    {
            //        if (pointerDatas[index_pointer].pushContact != null)
            //        {
            //            PointerUp(pointerDatas[index_pointer].pushContact);

            //            if (results.Count != 0 && pointerDatas[index_pointer].pushContact == results[0].gameObject)
            //            {
            //                PointerClick(pointerDatas[index_pointer].pushContact);
            //            }
            //        }
            //    }
            //    ped.pointerPressRaycast = new RaycastResult();
            //    pointerDatas[index_pointer].dragContact = null;
            //    pointerDatas[index_pointer].isDrag = false;


            //    pointerDatas[index_pointer].isSelectOut = false;
            //    pointerDatas[index_pointer].isPress = false;

            //    pointerDatas[index_pointer].pushContact = null;

            //    if (results.Count != 0)
            //    {
            //        if (!isSamePointer && pointerDatas[index_pointer].previousContact != null && results[0].gameObject != pointerDatas[index_pointer].previousContact)
            //        {
            //            PointerExit(pointerDatas[index_pointer].previousContact);
            //        }

            //        pointerDatas[index_pointer].previousContact = results[0].gameObject;
            //    }
            //    else
            //    {
            //        if (!isSamePointer && pointerDatas[index_pointer].previousContact != null)
            //        {
            //            PointerExit(pointerDatas[index_pointer].previousContact);
            //        }

            //        pointerDatas[index_pointer].previousContact = null;
            //    }
            //}
            //else if (results.Count != 0)
            //{
            //    if (pointerDatas[index_pointer].dragContact != null && !pointerDatas[index_pointer].isDrag)
            //    {
            //        //if ((pointerDatas[index_pointer].pressStartPos - (endPos - transform.position)).sqrMagnitude >= dragThreshold)
            //        if ((1f - Vector3.Dot(pointerDatas[index_pointer].pressStartDir, pointerDatas[index_pointer].pointer_line.transform.forward)) >= dragThreshold)
            //        {
            //            pointerDatas[index_pointer].isDrag = true;
            //            ped.pointerPressRaycast = ((results.Count != 0) ? results[0] : new RaycastResult());
            //            BeginDrag(pointerDatas[index_pointer].dragContact, ped);
            //        }
            //    }

            //    if (pointerDatas[index_pointer].isDrag && pointerDatas[index_pointer].dragContact != null)
            //    {
            //        Drag(pointerDatas[index_pointer].dragContact, ped);
            //        ped.pointerPressRaycast = ((results.Count != 0) ? results[0] : new RaycastResult());
            //    }

            //    if (pointerDatas[index_pointer].previousContact != results[0].gameObject)
            //    {

            //        if (pointerDatas[index_pointer].isPress)
            //        {
            //            if (pointerDatas[index_pointer].isSelectOut && results[0].gameObject == pointerDatas[index_pointer].pushContact)
            //            {
            //                pointerDatas[index_pointer].isSelectOut = false;
            //            }
            //            else if (!pointerDatas[index_pointer].isSelectOut && results[0].gameObject != pointerDatas[index_pointer].pushContact)
            //            {
            //                pointerDatas[index_pointer].isSelectOut = true;
            //            }
            //        }

            //        if (!isSamePointer && pointerDatas[index_pointer].previousContact != null)
            //        {
            //            PointerExit(pointerDatas[index_pointer].previousContact);
            //        }

            //        PointerEnter(results[0].gameObject);

            //        pointerDatas[index_pointer].previousContact = results[0].gameObject;
            //    }
            //}
            //else
            //{
            //    if (pointerDatas[index_pointer].dragContact != null && !pointerDatas[index_pointer].isDrag)
            //    {
            //        //if ((pointerDatas[index_pointer].pressStartPos - (endPos- transform.position)).sqrMagnitude >= dragThreshold)
            //        if ((1f - Vector3.Dot(pointerDatas[index_pointer].pressStartDir, pointerDatas[index_pointer].pointer_line.transform.forward)) >= dragThreshold)
            //        {
            //            pointerDatas[index_pointer].isDrag = true;
            //            BeginDrag(pointerDatas[index_pointer].dragContact, ped);
            //        }
            //    }

            //    if (pointerDatas[index_pointer].isDrag && pointerDatas[index_pointer].dragContact != null)
            //    {
            //        Drag(pointerDatas[index_pointer].dragContact, ped);
            //    }


            //    if (pointerDatas[index_pointer].previousContact != null)
            //    {
            //        if (pointerDatas[index_pointer].isPress)
            //        {
            //            pointerDatas[index_pointer].isSelectOut = true;
            //        }

            //        if (!isSamePointer)
            //        {
            //            PointerExit(pointerDatas[index_pointer].previousContact);
            //        }

            //        pointerDatas[index_pointer].previousContact = null;
            //    }
            //}
        }

        public void PointerEnter(GameObject target)
        {
            //IPointerEnterHandler[] pointerEnterHandlers = target.GetComponentsInParent<IPointerEnterHandler>();
            //if (pointerEnterHandlers.Length >= 1)
            //{
            //    for (int i = 0; i < pointerEnterHandlers.Length; i++)
            //    {
            //        pointerEnterHandlers[i].OnPointerEnter(new PointerEventData(null));
            //    }
            //}
        }

        public void PointerExit(GameObject target)
        {
            //IPointerExitHandler[] pointerExitHandlers = target.GetComponentsInParent<IPointerExitHandler>();

            //if (pointerExitHandlers.Length >= 1)
            //{
            //    for (int i = 0; i < pointerExitHandlers.Length; i++)
            //    {
            //        pointerExitHandlers[i].OnPointerExit(new PointerEventData(null));
            //    }
            //}
        }

        public void PointerDown(GameObject target, PointerEventData m_ped)
        {
            //IPointerDownHandler[] pointerDownHandlers = target.GetComponents<IPointerDownHandler>();
            //if (pointerDownHandlers.Length >= 1)
            //{
            //    for (int i = 0; i < pointerDownHandlers.Length; i++)
            //    {
            //        pointerDownHandlers[i].OnPointerDown(m_ped);
            //    }
            //}
            //else if (target.transform.parent != null)
            //{
            //    PointerDown(target.transform.parent.gameObject, m_ped);
            //}
        }

        public void PointerUp(GameObject target)
        {
            //IPointerUpHandler[] pointerUpHandlers = target.GetComponents<IPointerUpHandler>();
            //if (pointerUpHandlers.Length >= 1)
            //{
            //    for (int i = 0; i < pointerUpHandlers.Length; i++)
            //    {
            //        pointerUpHandlers[i].OnPointerUp(new PointerEventData(null));
            //    }
            //}
            //else if (target.transform.parent != null)
            //{
            //    PointerUp(target.transform.parent.gameObject);
            //}
        }

        public void PointerClick(GameObject target)
        {
            //IPointerClickHandler[] pointerClickHandlers = target.GetComponents<IPointerClickHandler>();

            //if (pointerClickHandlers.Length >= 1)
            //{
            //    for (int i = 0; i < pointerClickHandlers.Length; i++)
            //    {
            //        pointerClickHandlers[i].OnPointerClick(new PointerEventData(null));
            //    }

            //    SetSelectedGameObject(target);
            //}
            //else if (target.transform.parent != null)
            //{
            //    PointerClick(target.transform.parent.gameObject);
            //}
        }

        public GameObject GetDrag(GameObject target, PointerEventData m_ped)
        {
            //IInitializePotentialDragHandler dragHandler = target.GetComponent<IInitializePotentialDragHandler>();
            //if (dragHandler != null)
            //{
            //    dragHandler.OnInitializePotentialDrag(m_ped);
            //    return target;
            //}
            //else if (target.transform.parent != null)
            //{
            //    return GetDrag(target.transform.parent.gameObject, m_ped);
            //}
            //else
            //{
            return null;
            //}
        }

        public void BeginDrag(GameObject target, PointerEventData m_ped)
        {
            //IBeginDragHandler dragHandler = target.GetComponent<IBeginDragHandler>();
            //if (dragHandler != null)
            //{
            //    dragHandler.OnBeginDrag(m_ped);
            //}
        }

        public void Drag(GameObject target, PointerEventData m_ped)
        {
            //IDragHandler dragHandler = target.GetComponent<IDragHandler>();
            //if (dragHandler != null)
            //{
            //    dragHandler.OnDrag(m_ped);
            //}
        }

        public void EndDrag(GameObject target, PointerEventData m_ped)
        {
            //IEndDragHandler dragHandler = target.GetComponent<IEndDragHandler>();
            //if (dragHandler != null)
            //{
            //    dragHandler.OnEndDrag(m_ped);
            //}
        }

        public void SetState(PointerState setState)
        {
            //state = setState;
            //if (state == PointerState.Hide)
            //{
            //    poses[0].transform.Find("Model").gameObject.SetActive(false);
            //}
            //else
            //{
            //    poses[0].transform.Find("Model").gameObject.SetActive(true);
            //}
        }

        [Serializable]
        [SerializeField]
        class PointerData
        {
            public GameObject pose;

            public GameObject pointer_ball;
            public GameObject pointer_line;

            public GameObject previousContact = null;
            public GameObject pushContact = null;
            public GameObject dragContact = null;

            public bool isPress = false;
            public bool isSelectOut = false;
            public bool isDrag = false;

            public int index_lastCanvas = 0;

            //public Vector3 pressStartPos = Vector3.zero;
            public Vector3 pressStartDir = Vector3.forward;
        }


        public void SetSelectedGameObject(GameObject target)
        {
            ISelectHandler[] selectHandlers = target.GetComponents<ISelectHandler>();

            if (selectContact != null)
            {
                IDeselectHandler[] dropHandlers = selectContact.GetComponents<IDeselectHandler>();
                for (int i = 0; i < dropHandlers.Length; i++)
                {
                    dropHandlers[i].OnDeselect(new PointerEventData(null));
                }
            }

            if (selectHandlers.Length >= 1)
            {
                selectContact = target;
                for (int i = 0; i < selectHandlers.Length; i++)
                {
                    selectHandlers[i].OnSelect(new PointerEventData(null));
                }
            }
        }

        public void DeselectButton()
        {
            StartCoroutine(initSelectContact());
        }

        IEnumerator initSelectContact()
        {
            yield return new WaitForSeconds(0.5f);
            selectContact = null;
        }
    }
}