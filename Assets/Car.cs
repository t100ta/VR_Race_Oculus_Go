using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//車
public class Car : MonoBehaviour
{
    Rigidbody rb;      //リキッドボディ
    GameObject camera; //カメラ

    //初期化
    void Start()
    {
        this.rb = GetComponent<Rigidbody>();
        this.camera = GameObject.Find("Main Camera");
    }

    private void Update()
    {
        OVRInput.Update();
    }

    //定期的に呼ばれる
    void FixedUpdate()
    {

#if UNITY_ANDROID && !UNITY_EDITOR
    OVRInput.FixedUpdate();

    Vector2 vector = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad, OVRInput.Controller.RTrackedRemote);
    float x = vector.x;
    float z = vector.y;
    if (x < 180) x += 720;
    if (z < 180) z += 720;
    float h = (Math.Min((360f - z), 45f) / 45f) * 1f;
    float v = (Math.Min((x - 360f), 10f) / 10f) * 20f;

#else   

        //VR用の車操作
        float x = camera.transform.localEulerAngles.x;
        float z = camera.transform.localEulerAngles.z;
        if (x < 180) x += 360;
        if (z < 180) z += 360;
        float h = (Math.Min((360f - z), 45f) / 45f) * 1f;
        float v = (Math.Min((x - 360f), 10f) / 10f) * 20f;

#endif


        //キーボード用の車操作
        if (h == 0f && v == 0f)
        {
            h = Input.GetAxis("Horizontal") * 1f;
            v = Input.GetAxis("Vertical") * 20f;
        }

        //車の向きと加速度の指定
        if (v != 0f)
        {
            this.gameObject.transform.Rotate(0, (v > 0) ? h : -h, 0);
        }
        this.rb.velocity = this.gameObject.transform.rotation *
            new Vector3(0, 0, v);
    }
}
