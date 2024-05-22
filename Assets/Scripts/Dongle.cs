using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    public bool isMerge;
    public int level;
    public bool isDrag;
    public ParticleSystem effect;
    public GameManager gameManager;

    Rigidbody2D rigid;
    Animator animator;
    CircleCollider2D circle;
  
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        circle = GetComponent<CircleCollider2D>();
    }
    private void OnEnable()
    {
        animator.SetInteger("Level",level);
    }

    void Update()
    {
        if (isDrag)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // x축 경계 설정
            float leftBorder = -4.2f + transform.localScale.x / 2f;
            float rightBorder = 4.2f - transform.localScale.x / 2f;

            if (mousePos.x < leftBorder)
            {
                mousePos.x = leftBorder;
            }
            else if (mousePos.x > rightBorder)
            {
                mousePos.x = rightBorder;
            }

            mousePos.z = 0;
            mousePos.y = 8;
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f);
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Dongle")
        {
            Dongle other = collision.gameObject.GetComponent<Dongle>();
            // 동글 합치기
            if (level == other.level && !isMerge && !other.isMerge && level < 7)
            {
                float myX = transform.position.x;
                float myY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;
                // 1. 내가 아래에 있을 때
                // 2. 동일한 높이, 오른쪽에 있을 때

                if (myY < otherY || (myY == otherY && myX > otherX))
                {
                    // 합쳐지며 숨기기
                    other.Hide(transform.position);
                    // 레벨업
                    LevelUp();
                }

            }
        }
    }
    
    public void Hide(Vector3 targetPos)
    {
        isMerge = true;
        // 충돌 시 합쳐져짐 -> 특성 끄기
        rigid.simulated = false;
        circle.enabled = false;

        StartCoroutine(HideRoutine(targetPos));

    }
    IEnumerator HideRoutine(Vector3 targetPos)
    {
        int frameCount = 0;

        while(frameCount < 20)
        {
            frameCount++;
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.8f);
            yield return null; // 1프레임 쉬기
        }

        isMerge = false;
        gameObject.SetActive(false);
    }
    
    void LevelUp()
    {
        isMerge = true;

        rigid.velocity = Vector2.zero; // 속도 0
        rigid.angularVelocity = 0; // 회전속도 0

        StartCoroutine(LevelUpRoutine());
    }
    IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        animator.SetInteger("Level", level + 1);
        EffectPlay();

        // 애니메이션 적용 시간 때문에 약간의 시간차 적용
        yield return new WaitForSeconds(0.3f);
        level++;

        gameManager.maxLevel = Mathf.Max(level, gameManager.maxLevel);

        isMerge = false;
    }

    void EffectPlay()
    {
        effect.transform.position = transform.position;
        effect.transform.localScale = transform.localScale;
        effect.Play();
    }
}


