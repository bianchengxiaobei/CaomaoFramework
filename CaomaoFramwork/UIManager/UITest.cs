using System;
using System.Collections.Generic;
using CaomaoFramework;
public class UITest : UIBase
{
    public UITest()
    {
        this.mResName = "Guis/UITest";
        this.mResident = false;
    }
    public override void Init()
    {
        EventDispatch.AddListener("test", this.Show);
    }

    public override void OnDisable()
    {
        
    }

    public override void OnEnable()
    {
        
    }

    public override void Realse()
    {
        
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