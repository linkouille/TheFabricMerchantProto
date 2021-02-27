using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interface : MonoBehaviour
{

    [SerializeField] private PlayerMovement pM;
    [SerializeField] private InputHolder iH;


    private void OnGUI()
    {
        //Debug Values
        GUI.Box(new Rect(0, 0, 150, 160), "Velocity :" + pM.getVel().ToString() + "\nIsGrounded :" + pM.IsGrounded()
            + "\nInputHolder" + (iH.useVoice ? "(Voice)" : "(KeyBoard)") + "\ninput : " + iH.input + "\nuseB : " + iH.useB + "\ninvB : " + iH.invB 
            + "\njumpB : " + iH.jumpB + "\njumpBUP : " + iH.jumpBUP + "\njumpBP : " + iH.jumpBP);
    }
}
