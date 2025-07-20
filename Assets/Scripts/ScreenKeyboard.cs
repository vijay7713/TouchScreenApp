using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class ScreenKeyboard : MonoBehaviour
{
    private Process process;

    InputField field;

    //private void Start()
    //{

    //}


    //void Update()
    //   {
    //	try
    //	{
    //		if (process.HasExited)
    //		{
    //           }
    //		else
    //		{
    //               Debug.Log("Process running");
    //           }
    //	}
    //	catch (System.Exception ex)
    //	{
    //           Console.WriteLine(ex);
    //       }
    //   }

    public void OnInputFieldSelect()
    {
        process = System.Diagnostics.Process.Start("OSK.exe");

    }
}
