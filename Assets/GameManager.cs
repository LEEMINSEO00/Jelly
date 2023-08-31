using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int jelatin;
    public int gold;

    public Text jelatin_text;
    public Text gold_text;

    public int[] jelly_goldlist; // 젤리의 가격 저장하는 배열
    public int max_gold; // 최대 골드

    public int max_jelatin; //최대 젤라틴


    // SmoothStep : 숫자가 변환되는 사이에 적절한 애니메이션을 추가해줌
    // Format : 숫자 텍스트의 표현식을 지정해줌
    public RuntimeAnimatorController[] level_ac; // Animator 변경을 관리하기 위해


    public Sprite[] jelly_spritelist;
    public string[] jelly_namelist;
    public int[] jelly_jelatinlist;

    public Text page_text;
    public Image unlock_group_jelly_img;
    public Text unlock_group_gold_text;
    public Text unlock_group_name_text;

    int page;

    public GameObject lock_group;
    public Image lock_group_jelly_img;
    public Text lock_group_jelatin_text;

    public bool[] jelly_unlock_list;

    public bool isLive;

    public static GameManager instance;

    public Text num_sub_text; //[업그레이드 시스템_Start]
    public Text num_btn_text;
    public Button num_btn;

    public Text click_sub_text;
    public Text click_btn_text;
    public Button click_btn;

    public int num_level;
    public int click_level;

    public int[] num_gold_list;
    public int[] click_gold_list;


    //[업그레이드 시스템_End]

    public void ChangeAc(Animator anim, int level)
    {
        anim.runtimeAnimatorController = level_ac[level - 1];
    } //ChangeAc() : Jelly에서 Animator 객체와 level을 받아와 runtimeAnimatorController로 해당 젤리 레벨에 따라 Animator를 변경한다.
      //level_ac 배열의 최대 크기 3, level 값은 1~3, level 값이 배열의 크기를 벗어나지 않도록 -1을 해준다.
    void LateUpdate()
    {
        jelatin_text.text = string.Format("{0:n0}", (int)Mathf.SmoothStep(float.Parse(jelatin_text.text), jelatin, 1f));
        gold_text.text = string.Format("{0:n0}", (int)Mathf.SmoothStep(float.Parse(gold_text.text), gold, 1f));
    }
    public bool isSell;

    public List<GameObject> jelly_list = new List<GameObject>();
    public List<Data> jelly_data_list = new List<Data>();

    public GameObject data_manager_obj;

    DataManager data_manager;

    void Start()
    {
        Invoke("LoadData", 0.5f);
    }

    void LoadData()
    {
        //[업그레이드 시스템_블로그 참고]
        jelatin_text.text = jelatin.ToString();
        gold_text.text = gold.ToString();
        unlock_group_gold_text.text = jelly_goldlist[0].ToString();
        lock_group_jelatin_text.text = jelly_jelatinlist[0].ToString();



        lock_group.gameObject.SetActive(!jelly_unlock_list[page]);

        //[업그레이드 시스템_Start]
        num_sub_text.text = "젤리 수용량 " + num_level * 2;
            if (num_level >= 5) num_btn.gameObject.SetActive(false);
            else num_btn_text.text = string.Format("{0:n0}", num_gold_list[num_level]);

        click_sub_text.text = "클릭 생산량 X " + click_level;
            if (click_level >= 5) click_btn.gameObject.SetActive(false);
            else click_btn_text.text = string.Format("{0:n0}", click_gold_list[click_level]);
        //[업그레이드 시스템_End]

        for (int i = 0; i < jelly_data_list.Count; ++i)
        {
            GameObject obj = Instantiate(prefab, jelly_data_list[i].pos, Quaternion.identity);
            Jelly jelly = obj.GetComponent<Jelly>();
            jelly.id = jelly_data_list[i].id;
            jelly.level = jelly_data_list[i].level;
            jelly.exp = jelly_data_list[i].exp;
            jelly.GetComponent<SpriteRenderer>().sprite = jelly_spritelist[jelly.id];
            jelly.anim.runtimeAnimatorController = level_ac[jelly.level - 1];
            obj.name = "Jelly " + jelly.id;

            jelly_list.Add(obj);
        }

    }

    void OnApplicationQuit()
    {
        data_manager.JsonSave();
    }

    void Awake()
    {
        instance = this;

        jelly_anim = jelly_panel.GetComponent<Animator>();
        plant_anim = plant_panel.GetComponent<Animator>();

        isLive = true;

  /*      PlayerPrefs.SetInt(0.ToString(), 1);*/

        isSell = false;

 

        jelatin_text.text = jelatin.ToString();
        gold_text.text = gold.ToString();

        unlock_group_gold_text.text = jelly_goldlist[0].ToString();
        lock_group_jelatin_text.text = jelly_jelatinlist[0].ToString();

        page = 0;
        jelly_unlock_list = new bool[12];
        /*        for (int i = 0; i < 12; ++i)
                    if (PlayerPrefs.HasKey(i.ToString()))
                        jelly_unlock_list[i] = true;

                lock_group.gameObject.SetActive(!jelly_unlock_list[page]);*/

        data_manager = data_manager_obj.GetComponent<DataManager>();

    }

    public void CheckSell() //젤리 판매
    {
        isSell = isSell == false;
    }
    public void GetGold(int id, int level, GameObject obj)
    {
        gold += jelly_goldlist[id] * level;

        if (gold > max_gold)
            gold = max_gold;

        jelly_list.Remove(obj);
    } //골드판매
    public void GetJelatin(int id, int level)
    {
        jelatin += (id + 1) * level * click_level; //젤리종류(id) 레벨(0~2)에 비례해서 젤라틴 얻기 //[업그레이드 시스템] click_level 추가

        if (jelatin > max_jelatin)
            jelatin = max_jelatin;
    }//젤라틴 얻기


/*   //젤리 구매할 때 페이지 변화

    //모든 젤리의 이미지, 이름, 구매시 필요한 젤라틴값을 저장하는 배열
    public Sprite[] jelly_spritelist; 
    public string[] jelly_namelist;
    public int[] jelly_jelatinlist;
    
    //해금 및 구매 페이지에 나올 젤리 이미지, 골드(가격) 텍스트, 젤리 이름 텍스트에 대한 변수.
    public Text page_text;
    public Image unlock_group_jelly_img;
    public Text unlock_group_gold_text;
    public Text unlock_group_name_text;

    int page;//현재 페이지에 대한 변수

    public GameObject lock_group;
    public Image lock_group_jelly_img;
    public Text lock_group_jelatin_text;

    bool[] jelly_unlock_list;*/


    public void PageUp()
    {
        if (page >= 11) return;

        ++page;
        ChangePage();
    }

    public void PageDown()
    {
        if (page <= 0) return;

        --page;
        ChangePage();
    }

    public void Unlock()
    {
        if (jelatin < jelly_jelatinlist[page]) return;

        jelly_unlock_list[page] = true;
        ChangePage();

        jelatin -= jelly_jelatinlist[page];

 /*       PlayerPrefs.SetInt(page.ToString(), 1);//언락이 되면 해당 페이지에 키값이 있다고 설정*/
    }

    void ChangePage()
    {
        lock_group.gameObject.SetActive(!jelly_unlock_list[page]);//jelly unlock이 true이면, lock_group 비활성화. jelly unlock이 false이면 lock_group활성화

        page_text.text = string.Format("#{0:00}", (page + 1));

        if (lock_group.activeSelf == true)//lock_group이 활성화되어있다. 잠금되어있다.
        {
            lock_group_jelly_img.sprite = jelly_spritelist[page];
            lock_group_jelatin_text.text = string.Format("{0:n0}", jelly_jelatinlist[page]);

            lock_group_jelly_img.SetNativeSize();
        }
        else
        {
            unlock_group_jelly_img.sprite = jelly_spritelist[page];
            unlock_group_name_text.text = jelly_namelist[page];
            unlock_group_gold_text.text = string.Format("{0:n0}", jelly_goldlist[page]);

            unlock_group_jelly_img.SetNativeSize();
        }
    }//page에 따라 배열값을 불러와 그 페이지에 해당하는 페이지텍스트, 젤리이미지, 젤리이름, 골드를 표시해주는 함수.

    //ButtonPanel은 Jelly버튼과 망치버튼(업그레이드버튼)을 "눌렀을 때", "애니메이션이 재생(판넬이 올라갔다 내려갔다)"되도록 하는 스크립트이다.
    //옵션 버튼을 눌렀을 때 게임(모든 요소)의 활성화 비활성화 상태를 조정한다.
    //판넬이 올라와있을 때는 클리커기능과 젤리드레그 위치이동, 판매 불가능하게 하는 기능.

    //UI이미지와 애니메이터를 저장할 변수를 생성하였다.
    public Image jelly_panel;
    public Image plant_panel;
    public Image option_panel;

    //위의 세 변수는 Inspector창에서 오브젝트를 끌어와 초기화하였다.

    Animator jelly_anim;
    Animator plant_anim;



/*    void Awake()
    {
        jelly_anim = jelly_panel.GetComponent<Animator>();
        plant_anim = plant_panel.GetComponent<Animator>();

    }*/

    //Jelly버튼이나 Plant버튼을 눌렀을 때 나오는 패널의 상태를 저장할 변수를 생성하였다.
    bool isJellyClick;//true: 패널이 올라와 있는 상태, false: 패널이 내려가있는 상태
    bool isPlantClick;
    //문제는 업그레이드 패널이 올라와 있으면 bool값이 true로 설정되어야 하는데, 그렇지 않은 경우(업그레이드 버튼 누른 다음, 젤리 버튼을 누른 경우.)가 존재한다. 
    public void ClickJellyBtn()
    {

        if (isPlantClick)
        {
            plant_anim.SetTrigger("doHide");
            isPlantClick = false;
            isLive = true;
        }//업그레이드 패널이 올라가있으면 내려준다. (젤리 패널을 올려야하니까.)


        if (isJellyClick)
            jelly_anim.SetTrigger("doHide");//패널이 올라가 있으면 내린다.
        else
            jelly_anim.SetTrigger("doShow");//패널이 내려가 있으면 올린다.

        isJellyClick = !isJellyClick;//상태 전환 (패널이 올라가 있는 상태에서 버튼 누름 => 내려간 것으로, 패널이 내려가 있는 상태에서 버튼 누름 => 올라간 것으로)
        isLive = !isLive;
    }

    public void ClickPlantBtn()
    {

        if (isJellyClick)
        {
            jelly_anim.SetTrigger("doHide");
            isJellyClick = false;
            isLive = true;
        }

        if (isPlantClick)
        {
            plant_anim.SetTrigger("doHide");
            isPlantClick = false;
        }
        else
        {
            plant_anim.SetTrigger("doShow");
            isPlantClick = true;
        }
        isLive = !isLive;
    }

    //ESC키를 눌렀을 때 다른 패널이 열려있지 않으면, 옵션 패널 활성화/비활성화하기 위한 함수
    bool isOption;

    void Option()
    {
        isOption = !isOption;
        isLive = !isLive;

        option_panel.gameObject.SetActive(isOption);//option_panel은 Option Panel오브젝트임. SetActive함수는 boolean값을 받아 오브젝트를 활성화, 비활성화시키는 함수이다. 즉 isOption값에 따라 실제로 오브젝트의 활성화여부를 조정하는 코드임.
        Time.timeScale = isOption == true ? 0 : 1; //timescale은 모든 게임 오브젝트가 어떤 시간 속도로 흘러가는지 표현하는 값이다. 옵션패널이 올라와있어 timescale이 0이면 일시 정지, 1이면 default값. 
    }

    //ESC키를 눌렀을 때 다른 패널(젤리, 업그레이드)이 열려있는 경우에는 해당 패널을 닫아주는 것으로.

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {

            if (isJellyClick) ClickJellyBtn();//패널이 올라와있으면 내려줌. 패널이 내려가있으면 아무것도 안함.
            else if (isPlantClick) ClickPlantBtn();
            else Option();

        }
    }

    public GameObject prefab;

    public void BuyJelly()
    {
        if (gold < jelly_goldlist[page] || jelly_list.Count >= num_level * 2) return; //[업그레이드 시스템] || jelly_list.Count >= num_level * 2 추가

        gold -= jelly_goldlist[page];

        GameObject obj = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        Jelly jelly = obj.GetComponent<Jelly>();
        obj.name = "Jelly" + page;
        jelly.id = page;
        //jelly.sprite_renderer.sprite = jelly_spritelist[page]; //[업그레이드 시스템] 원래: jelly.GetComponent<SpriteRenderer>().sprite = jelly_spritelist[page];
        jelly.GetComponent<SpriteRenderer>().sprite = jelly_spritelist[page]; //[업그레이드 시스템] 위의 수정문구로 시도하면 오류생겨서 원래 문장으로 시도

        jelly_list.Add(obj); //[업그레이드 시스템] 원래: Add(obj); 수정: Add.(jelly);이나 오류가 떠 원래대로
    }

    //[업그레이드 시스템]
    public void NumUpgrade()
    {
        if (gold < num_gold_list[num_level]) return;

        gold -= num_gold_list[num_level++];

        num_sub_text.text = "젤리 수용량 " + num_level * 2;

        if (num_level >= 5) num_btn.gameObject.SetActive(false);
        else num_btn_text.text = string.Format("{0:n0}", num_gold_list[num_level]);
    }

    public void ClickUpgrade()
    {
        if (gold < click_gold_list[click_level]) return;

        gold -= click_gold_list[click_level++];

        click_sub_text.text = "클릭 생산량 X " + click_level;

        if (click_level >= 5) click_btn.gameObject.SetActive(false);
        else click_btn_text.text = string.Format("{0:n0}", click_gold_list[click_level]);
    }

}