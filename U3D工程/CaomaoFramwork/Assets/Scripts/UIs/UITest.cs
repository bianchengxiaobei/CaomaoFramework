using System;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework;
using FairyGUI;
[Serializable]
public class UITest : UIBase
{
    public UITest()
    {
        this.mResName = "Guis/UITest";
        this.mResident = false;
    }
    public override void Init()
    {
        EventDispatch.AddListener("UITest", this.Show);
    }

    public override void OnDisable()
    {
        
    }

    public override void OnEnable()
    {
        
    }

    public override void Realse()
    {
        EventDispatch.RemoveListener("UITest", this.Show);
    }

    protected override void InitWidget()
    {
    }

    protected override void OnAddListener()
    {
        
    }

    protected override void OnRemoveListener()
    {
        
    }

    protected override void RealseWidget()
    {
       
    }
}