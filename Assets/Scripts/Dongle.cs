using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    public GameManager manager;
    public int Level;
    public bool isDrag;
    public bool isMerge;
   
   public Rigidbody2D rigid;
    CircleCollider2D circle;
    Animator anim;
    SpriteRenderer spriteRenderer;
    float deadTime;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        anim.SetInteger("Level", Level);
    }

    private void OnDisable()
    {
       // 동글 속성 초기화
        Level = 0;
        isDrag = false;
        isMerge = false;

        // 동글 트랜스폼 초기화
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.zero;

        // 동글 물리 초기화
        rigid.simulated = false;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        circle.enabled = true;

    }

    void Update()
    {
        if (isDrag)
        {
            Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // x축 경계 설정
            float LeftBorder = -4.2f + transform.localScale.x / 2f;
            float RightBorder = 4.2f - transform.localScale.x / 2f;

            if (mousepos.x < LeftBorder)
            {
                mousepos.x = LeftBorder;
            }
            else if (mousepos.x > RightBorder)
            {
                mousepos.x = RightBorder;
            }

            mousepos.y = 8;
            mousepos.z = 0;
            transform.position = Vector3.Lerp(transform.position, mousepos, 0.2f);

        }

    }
    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag = false;
        rigid.simulated = true;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Dongle")
        {
            Dongle other = collision.gameObject.GetComponent<Dongle>();

            if(Level == other.Level && !isMerge && !other.isMerge && Level < 7)
            {
                // 나와 상대편 위치 가져오기
                float meX = transform.position.x;
                float meY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;
                // 1. 내가 아래에 있을 때
                // 2. 동일한 높이일 때, 내가 오른쪽에 있을 때
                if(meY < otherY || (meY == otherY && meX > otherX))
                {
                    // 상대방은 숨기기
                    other.Hide(transform.position);
                    // 내가 레벨업
                    LevelUp();

                }
            }
        }
    }
    public void Hide(Vector3 targetpos)
    {
        isMerge = true;

        rigid.simulated = false;
        circle.enabled = false;

        StartCoroutine(HideRoutine(targetpos));
    }

    IEnumerator HideRoutine(Vector3 targetpos)
    {
        int framCount = 0;

        while (framCount < 20) {
            framCount++;
            if (targetpos != Vector3.up * 100) {
                transform.position = Vector3.Lerp(transform.position, targetpos, 0.5f);
            }
            else if(targetpos == Vector3.up * 100){
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.2f);
            }
            
                    
                    yield return null;
        }

        manager.score += (int)Mathf.Pow(2, Level);
        
        isMerge = false;
        gameObject.SetActive(false);
        
        
    }

    void LevelUp()
    {
        isMerge = true;

        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;

        StartCoroutine(LevelUpRoutine());
    }

    IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        anim.SetInteger("Level", Level + 1);

        yield return new WaitForSeconds(0.3f);
        Level++;

        manager.maxLevel = Mathf.Max(Level, manager.maxLevel);

        isMerge = false;
    }

     void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Finish")
        {
            deadTime += Time.deltaTime;

            if(deadTime > 2)
            {
                spriteRenderer.color = Color.red;
            }
            if(deadTime > 5)
            {
                manager.GameOver();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Finish")
        {
            deadTime = 0;
            spriteRenderer.color = Color.white;
        }
    }
}
