using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jelly : MonoBehaviour
{
    public int id; //젤리의 종류 , 초기값 0으로 설정
    public int level; // 젤리의 레벨 , 초기값 1으로 설정
    public float exp; // level이 오르는 기준
    public float required_exp; // level 오르는데 요구되는 경험치
    public float max_exp; // 레벨당 최고 경험치

    public GameManager game_manager; // GameManager 객체를 Jelly 스크립트 내에 추가( 재화정보 활용하기 위해)

    public int move_delay;  // 다음 이동까지의 딜레이 시간
    public int move_time;   // 이동 시간

    float speed_x;  // x축 방향 이동 속도
    float speed_y;  // y축 방향 이동 속도
    bool isWandering;
    bool isWalking;

    float pick_time; // 젤리가 집어들어질 때 

    public GameObject left_top; //경계
    public GameObject right_bottom;


    SpriteRenderer sprite;
    public Animator anim;

    GameObject shadow;
    float shadow_pos_y;

    public GameObject game_manager_obj;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        isWandering = false;
        isWalking = false;

        shadow = transform.Find("Shadow").gameObject;

        switch (id)
        {
            case 6: shadow_pos_y = -0.12f; break;
            case 3: shadow_pos_y = -0.14f; break;
            case 10: shadow_pos_y = -0.16f; break;
            case 11: shadow_pos_y = -0.05f; break;
            default: shadow_pos_y = -0.05f; break;
        }

        shadow.transform.localPosition = new Vector3(0, shadow_pos_y, 0);

        left_top = GameObject.Find("TopLeft").gameObject;
        right_bottom = GameObject.Find("BottomRight").gameObject;
        game_manager_obj = GameObject.Find("GameManager").gameObject;
        game_manager = game_manager_obj.GetComponent<GameManager>();

        isGetting = false;
    }

    void Move()
    {
        if(speed_x != 0)
            sprite.flipX = speed_x < 0;

        transform.Translate(speed_x, speed_y, speed_y);  // 젤리 이동
    }

    void FixedUpdate() 
    {
        if (!isWandering)
            StartCoroutine(Wander()); // 코루틴 실행
        if (isWalking)
            Move();

        float pos_x = transform.position.x;
        float pos_y = transform.position.y;

        if (pos_x < left_top.transform.position.x || pos_x > right_bottom.transform.position.x) //경계
            speed_x = -speed_x;
        if (pos_y > left_top.transform.position.y || pos_y < right_bottom.transform.position.y)
            speed_y = -speed_y;
    }

    IEnumerator Wander()
    {
        move_delay = 6;
        move_time = 3;
        
        // Translate로 이동할 시 Object가 텔레포트 하는 것을 방지하기 위해 Time.deltaTime을 곱해줌
        speed_x = Random.Range(-0.8f, 0.8f) * Time.deltaTime;
        speed_y = Random.Range(-0.8f, 0.8f) * Time.deltaTime;

        isWandering = true;

        yield return new WaitForSeconds(move_delay);

        isWalking = true;
        anim.SetBool("isWalk", true);	// 이동 애니메이션 실행

        yield return new WaitForSeconds(move_time);
        
        isWalking = false;
        anim.SetBool("isWalk", false);	// 이동 애니메이션 종료

        isWandering = false;
    }
    void OnMouseDown()
    {
        if (!game_manager.isLive) return;
        
        isWalking = false;
        anim.SetBool("isWalk", false);
        anim.SetTrigger("doTouch");
        //game_manager.GetJelatin(id, level);

        if (exp < max_exp) // 최대 경험치가 도달할때 까지 기능
            ++exp; // 클릭할때마다 경험치 증가

        if (game_manager.jelatin < 99999999)
            game_manager.jelatin += (id + 1) * level; //id 와 level에 비례하여 얻는 재화 증가
    }
    void Update()
    {
        if (exp < max_exp)
            exp += Time.deltaTime; //시간이 지날 때마다 경험치 증가

        if (exp > 50 * level && level < 3)
            game_manager.ChangeAc(anim, ++level);

        if (!isGetting)
            StartCoroutine(GetJelatin());
    }
    void OnMouseDrag() // 마우스 드래그
    {
        if (!game_manager.isLive) return;

        pick_time += Time.deltaTime; 

        if (pick_time < 0.1f) return; // 단순클릭, 드래그를 구분하기 위해

        isWalking = false;
        anim.SetBool("isWalk", false);
        anim.SetTrigger("doTouch");

        Vector3 mouse_pos = Input.mousePosition;
        Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(mouse_pos.x, mouse_pos.y, mouse_pos.y)); //ScreenToWorldPoint로 마우스의 위치를 월드 좌표계로 변경

        transform.position = point;
    }
    void OnMouseUp() //마우스를 놓으면
    {
        if (!game_manager.isLive) return;

        pick_time = 0;
        float pos_x = transform.position.x; //위치변경
        float pos_y = transform.position.y;

        if (game_manager.isSell) //젤리판매
        {
            game_manager.GetGold(id, level, this.gameObject);

            Destroy(gameObject);
        }

        if (pos_x < left_top.transform.position.x || pos_x > right_bottom.transform.position.x || // 만약 경계밖으로 나가면 위치 초기화
            pos_y > left_top.transform.position.y || pos_y < right_bottom.transform.position.y)
            transform.position = new Vector3(0, -1, 0);
    }

    int jelatin_delay;
    bool isGetting;

    IEnumerator GetJelatin()
    {
        jelatin_delay = 3;

        isGetting = true;

        if (game_manager.jelatin < 99999999)
            game_manager.jelatin += (id + 1) * level; //id 와 level에 비례하여 얻는 재화 증가

        yield return new WaitForSeconds(jelatin_delay);

        isGetting = false;
    }
}
