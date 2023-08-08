using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Appnori.Util;

public class ChangeStoreState : MonoBehaviour
{
   

    public Notifier<int> nowStore;
    [SerializeField] private List<Button> list_but;
    [SerializeField] private List<GameObject> list_type;
    
    void Start()
    {
       
        nowStore.Value = 0;
        nowStore.OnDataChanged += OnChanged;
        list_type.Add(transform.Find("Type_Dia").gameObject);
        list_type.Add(transform.Find("Type_Gold").gameObject);
        list_type.Add(transform.Find("Type_Thema").gameObject);
        list_but.Add(transform.Find("Button_Dia").GetComponentInChildren<Button>());
        list_but.Add(transform.Find("Button_Nang").GetComponentInChildren<Button>());
        list_but.Add(transform.Find("Button_Map").GetComponentInChildren<Button>());
        list_but[0].onClick.AddListener(() => SetChangeStoreState(0));
        list_but[1].onClick.AddListener(() => SetChangeStoreState(1));
        list_but[2].onClick.AddListener(() => SetChangeStoreState(2));



    }

    public void SetChangeStoreState(int now) // 버튼에 연결 
    {
        nowStore.Value = now;
    }

    private void OnChanged(int now) // 계속 상점 상태 확인후 내용을 변경할 함수
    {
        list_type.Find(x => x.activeSelf == true).SetActive(false);
        list_type[now].SetActive(true);
    }


    
   
}
