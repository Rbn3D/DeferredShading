﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;


public class TestGPUTrail : MonoBehaviour
{
    public DSRenderer cam;
    public GPUParticleWorld particleSet;
    public GPUParticleForce vectorField;
    public GPUParticleForce gravity;
    public GPUParticleEmitter emitter;
    public bool showGUI;
    public bool rotateByTime = true;
    public bool fixCamera = false;
    public int particlesParFrame = 52;



    void Start()
    {
    }

    void Update()
    {
        CameraControl();

        if (Input.GetKeyUp(KeyCode.Space))
        {
            showGUI = !showGUI;
        }
    }

    void CameraControl()
    {
        //Vector3 pos = Quaternion.Euler(0.0f, Time.deltaTime * -15.0f, 0) * cam.transform.position;
        Vector3 pos = cam.transform.position;
        if (Input.GetKeyUp(KeyCode.R)) { rotateByTime = !rotateByTime; }
        if (Input.GetKeyUp(KeyCode.F)) { fixCamera = !fixCamera; }
        if (Input.GetKeyUp(KeyCode.E)) { emitter.enabled = !emitter.enabled; }
        if (!fixCamera)
        {
            if (rotateByTime)
            {
                pos = Quaternion.Euler(0.0f, Time.deltaTime * -10.0f, 0) * pos;
            }
            if (Input.GetMouseButton(0))
            {
                float ry = Input.GetAxis("Mouse X") * 3.0f;
                float rxz = Input.GetAxis("Mouse Y") * 0.25f;
                pos = Quaternion.Euler(0.0f, ry, 0) * pos;
                pos.y += rxz;
            }
            {
                float wheel = Input.GetAxis("Mouse ScrollWheel");
                pos += pos.normalized * wheel * 4.0f;
            }
        }
        cam.transform.position = pos;
        cam.transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
    }

    void OnGUI()
    {
        float lineheight = 22.0f;
        float margin = 0.0f;
        float labelWidth = 130.0f;
        float x = 10.0f;
        float y = 10.0f;

        if (!showGUI) { return; }


        GUI.Label(new Rect(x, y, labelWidth, lineheight), "particles par frame:");
        GUI.TextField(new Rect(x + labelWidth, y, 50, lineheight), emitter.m_emit_count.ToString());
        emitter.m_emit_count = (int)GUI.HorizontalSlider(new Rect(x + labelWidth + 55, y, 100, lineheight), emitter.m_emit_count, 0, 32);
        y += lineheight + margin;

        y += 10.0f;

        {
            float cellsize = vectorField.m_vectorfield_cellsize.x;
            GUI.Label(new Rect(x, y, labelWidth, lineheight), "cell size:");
            GUI.TextField(new Rect(x + labelWidth, y, 50, lineheight), cellsize.ToString());
            cellsize = (float)GUI.HorizontalSlider(new Rect(x + labelWidth + 55, y, 100, lineheight), cellsize, 0.1f, 8.0f);
            vectorField.m_vectorfield_cellsize = Vector3.one * cellsize;
            y += lineheight + margin;
        }
        {
            GUI.Label(new Rect(x, y, labelWidth, lineheight), "strength:");
            GUI.TextField(new Rect(x + labelWidth, y, 50, lineheight), vectorField.m_strength_near.ToString());
            vectorField.m_strength_near = (float)GUI.HorizontalSlider(new Rect(x + labelWidth + 55, y, 100, lineheight), vectorField.m_strength_near, 0, 20);
            y += lineheight + margin;
        }
        {
            GUI.Label(new Rect(x, y, labelWidth, lineheight), "random diffuse:");
            GUI.TextField(new Rect(x + labelWidth, y, 50, lineheight), vectorField.m_random_diffuse.ToString());
            vectorField.m_random_diffuse = (float)GUI.HorizontalSlider(new Rect(x + labelWidth + 55, y, 100, lineheight), vectorField.m_random_diffuse, 0, 20);
            y += lineheight + margin;
        }
        {
            GUI.Label(new Rect(x, y, labelWidth, lineheight), "random seed:");
            GUI.TextField(new Rect(x + labelWidth, y, 50, lineheight), vectorField.m_random_seed.ToString());
            vectorField.m_random_seed = (float)GUI.HorizontalSlider(new Rect(x + labelWidth + 55, y, 100, lineheight), vectorField.m_random_seed, 0.1f, 20.0f);
            y += lineheight + margin;
        }
        {
            GUI.Label(new Rect(x, y, labelWidth, lineheight), "gravity:");
            GUI.TextField(new Rect(x + labelWidth, y, 50, lineheight), gravity.m_strength_near.ToString());
            gravity.m_strength_near = (float)GUI.HorizontalSlider(new Rect(x + labelWidth + 55, y, 100, lineheight), gravity.m_strength_near, 0.0f, 20.0f);
            y += lineheight + margin;
        }


        y += 10.0f;

        GUI.Label(new Rect(x, y, 300, lineheight), "mouse drag & wheel: move camera");
        y += lineheight + margin;

        GUI.Label(new Rect(x, y, 300, lineheight), "space: show / hide GUI");
        y += lineheight + margin;
        GUI.Label(new Rect(x, y, 300, lineheight), "R: camera rotation on / off");
        y += lineheight + margin;
    }
}
