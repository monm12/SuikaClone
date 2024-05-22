using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dongle lastDongle;
    public GameObject donglePrefab;
    public Transform dongleGroup;
    public GameObject effectPrefab;
    public Transform effectGroup;

    public int maxLevel;

    private void Awake()
    {
        Application.targetFrameRate = 60; // prefab -rigidbody - interplate로 설정하면 부드러운 화면
    }
    void Start()
    {
        NextDongle();
    }

    Dongle GetDongle()
    {
        // 이펙트 생성
        GameObject instantEffectObj= Instantiate(effectPrefab, effectGroup);
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();

        // 동글 생성
        GameObject instantDongleObj = Instantiate(donglePrefab, dongleGroup); // Instantiate(변수, 부모);
        Dongle instantDongle = instantDongleObj.GetComponent<Dongle>();
        instantDongle.effect = instantEffect;

        return instantDongle;
    }
    void NextDongle()
    {
        Dongle newDongle = GetDongle();
        lastDongle = newDongle;

        lastDongle.level = Random.Range(0, maxLevel); // level을 0-7값을 랜덤으로 선택
        lastDongle.gameObject.SetActive(true); // 프리팹을 비활성화 시킨 후 SetActive로 오브젝트 활성화

        StartCoroutine("WaitNext"); // 코루틴 제어, 파라미터 타입 - 그대로 or String
        lastDongle.gameManager = this;

    }
    // 코루틴 - 로직제어를 유니티에 맡김
    IEnumerator WaitNext()
    {
        while(lastDongle != null)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2.0f); // 2초 쉬고 아래 로직 실행

        NextDongle();
    }

    public void TouchDown()
    {
        if (lastDongle == null)
        {
            return;
        }

        lastDongle.Drag();
    }
    public void TouchUp()
    {
        if (lastDongle == null)
        {
            return;
        }
        lastDongle.Drop();

        lastDongle = null;
    }

}
