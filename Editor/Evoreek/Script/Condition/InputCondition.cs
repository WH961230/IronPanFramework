using UnityEngine;

/// <summary>
/// 输入条件
/// </summary>
[CreateAssetMenu(menuName = "Evoreek/Condition/InputCondition")]
public class InputCondition : GameCondition {
    public InputConditionOutput inputConditionOutput;
    public override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        inputConditionOutput = new InputConditionOutput();
    }

    public override IOutput GetResult() {
        inputConditionOutput.x = Input.GetAxis("Horizontal");
        inputConditionOutput.z = Input.GetAxis("Vertical");
        inputConditionOutput.mouseX = Input.GetAxis("Mouse X");
        inputConditionOutput.mouseY = Input.GetAxis("Mouse Y");
        
        inputConditionOutput.Shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        inputConditionOutput.Enter = Input.GetKeyDown(KeyCode.Return);
        inputConditionOutput.G = Input.GetKey(KeyCode.G);
        inputConditionOutput.F = Input.GetKey(KeyCode.F);
        inputConditionOutput.W = Input.GetKey(KeyCode.W);
        inputConditionOutput.S = Input.GetKey(KeyCode.S);
        inputConditionOutput.A = Input.GetKey(KeyCode.A);
        inputConditionOutput.D = Input.GetKey(KeyCode.D);
        inputConditionOutput.C = Input.GetKey(KeyCode.C);

        inputConditionOutput.LeftClick = Input.GetMouseButtonDown(0);
        inputConditionOutput.LeftLongClick = Input.GetMouseButton(0);
        inputConditionOutput.LeftUp = Input.GetMouseButtonUp(0);

        inputConditionOutput.RightClick = Input.GetMouseButtonDown(1);
        inputConditionOutput.RightLongClick = Input.GetMouseButton(1);
        inputConditionOutput.RightUp = Input.GetMouseButtonUp(1);

        inputConditionOutput.Space = Input.GetKeyDown(KeyCode.Space);
        inputConditionOutput.SpaceLong = Input.GetKey(KeyCode.Space);
        inputConditionOutput.ScrollWheel = Input.mouseScrollDelta.y;

        inputConditionOutput.alpha0 = Input.GetKeyDown(KeyCode.Alpha0);
        inputConditionOutput.alpha1 = Input.GetKeyDown(KeyCode.Alpha1);
        inputConditionOutput.alpha2 = Input.GetKeyDown(KeyCode.Alpha2);
        inputConditionOutput.alpha3 = Input.GetKeyDown(KeyCode.Alpha3);
        inputConditionOutput.alpha4 = Input.GetKeyDown(KeyCode.Alpha4);
        inputConditionOutput.alpha5 = Input.GetKeyDown(KeyCode.Alpha5);
        inputConditionOutput.alpha6 = Input.GetKeyDown(KeyCode.Alpha6);
        inputConditionOutput.alpha7 = Input.GetKeyDown(KeyCode.Alpha7);
        inputConditionOutput.alpha8 = Input.GetKeyDown(KeyCode.Alpha8);
        return inputConditionOutput;
    }
}

/// <summary>
/// 输入条件输出信息
/// </summary>
public class InputConditionOutput : IOutput {
    public float x;
    public float y;
    public float z;
    public bool G;
    public bool F;
    public bool W;
    public bool S;
    public bool A;
    public bool D;
    public bool C;
    public bool Enter;
    public float mouseX;
    public float mouseY;
    public bool Shift;
    public bool Space;
    public bool SpaceLong;
    public bool LeftClick;//短按左键一次
    public bool LeftLongClick;//长按左键
    public bool LeftUp;
    public bool RightClick;//短按右键一次
    public bool RightLongClick;//长按右键
    public bool RightUp;
    public float ScrollWheel;
    
    public bool alpha0;
    public bool alpha1;
    public bool alpha2;
    public bool alpha3;
    public bool alpha4;
    public bool alpha5;
    public bool alpha6;
    public bool alpha7;
    public bool alpha8;
}