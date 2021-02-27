using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows.Speech;

public class InputHolder : MonoBehaviour
{
    public bool useVoice;

    [SerializeField] private string HAxis;
    [SerializeField] private string VAxis;

    [SerializeField] private string JumpButon;
    [SerializeField] private string UseButon;
    [SerializeField] private string InvButon;
    

    public Vector3 input;

    public bool jumpB;
    public bool jumpBUP;
    public bool jumpBP;
    public bool useB;
    public bool invB;

    [SerializeField] private float downTimedetection = 0.1f;
    [SerializeField] private float upTimedetection = 0.1f;
    

    [SerializeField] private Dictionary<string, UnityAction> gramar;
    [SerializeField] private ConfidenceLevel confidence = ConfidenceLevel.Medium;
    
    private DictationRecognizer dR;

    private string word;

    private bool stopMove = false;

    private bool dRState = false;

    private void Awake()
    {

        gramar = new Dictionary<string, UnityAction>();
        string[] keywords = { "gauche", "droite", "haut", "bas", "jump", "stop" , "utiliser", "inventaire"};       
        gramar.Add(keywords[0], GoLeft);
        gramar.Add(keywords[1], GoRight);
        gramar.Add(keywords[2], GoUp);
        gramar.Add(keywords[3], GoDown);
        gramar.Add(keywords[4], PressJump);
        gramar.Add(keywords[5], Stop);
        gramar.Add(keywords[6], PressUse);
        gramar.Add(keywords[7], PressInv);
        

        dR = new DictationRecognizer(confidence);
        

        dR.DictationHypothesis += (text) =>
        {
            
            string[] textArray = text.Split(' ');

            foreach (string s in textArray)
            {
                if (IsInside<string>(keywords, s))
                {
                    Debug.Log(s + " is Valid");
                    gramar[s].Invoke();
                }
            }
        };


    }

    private bool IsInside<T>(T[] array, T element)
    {
        foreach (T e in array)
        {
            if (e.Equals(element)){
                return true;
            }
        }

        return false;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            useVoice = !useVoice;

        if (useVoice)
        {
            InputVoice();
        }
        else
        {
            InputKeyBoard();
        }
    }

    private void InputKeyBoard()
    {
        if (dR != null && dRState)
        {
            dR.Stop();
            dRState = false;
        }

        input = Vector3.up * Input.GetAxis(VAxis) + Vector3.right * Input.GetAxis(HAxis);

        jumpB = Input.GetButtonDown(JumpButon);
        jumpBUP = Input.GetButtonUp(JumpButon);
        jumpBP = Input.GetButton(JumpButon);
        useB = Input.GetButtonDown(UseButon);
        invB = Input.GetButtonDown(InvButon);
        
    }


    private void InputVoice()
    {
        if (dR != null && !dRState)
        {
            dR.Start();
            dRState = true;
            Debug.Log("Voice Recognizer now running");
        }
    }
    

    IEnumerator SlowMoForSecond(float time)
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(time);
        Time.timeScale = 1f;

    }

    private void OnApplicationQuit()
    {
        if (dR != null && dRState)
        {
            dR.Stop();
        }

    }

    private void GoLeft()
    {
        StopCoroutine("PseudoAxisX");
        StartCoroutine("PseudoAxisX", -1);
    }
    private void GoRight()
    {
        StopCoroutine("PseudoAxisX");
        StartCoroutine("PseudoAxisX", 1);
    }
    private void GoUp()
    {
        StopCoroutine("PseudoAxisY");
        StartCoroutine("PseudoAxisY", 1);
    }
    private void GoDown()
    {
        StopCoroutine("PseudoAxisY");
        StartCoroutine("PseudoAxisY", -1);
    }
    private void Stop()
    {
        StopCoroutine("PseudoAxisX");
        StopCoroutine("PseudoAxisY");
        input = Vector3.zero;
    }

    IEnumerator PseudoAxisX(int dir)
    {
        while(Mathf.Abs(input.x) <= Mathf.Abs(dir))
        {
            input.x = Mathf.Lerp(input.x,dir,0.5f);
            
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator PseudoAxisY(int dir)
    {
        while (input.y != dir)
        {
            input.y = Mathf.Lerp(input.y, dir, 0.5f);

            yield return new WaitForEndOfFrame();
        }
    }

    private void PressJump()
    {
        StartCoroutine("IPressJump", 1);

    }

    IEnumerator IPressJump(float time)
    {
        jumpB = true;
        jumpBP = true;
        yield return new WaitForSeconds(Time.deltaTime);
        jumpB = false;
        yield return new WaitForSeconds(time);
        jumpBP = false;
        jumpBUP = true;
        yield return new WaitForSeconds(Time.deltaTime);
        jumpBUP = false;
    }

    private void PressUse()
    {
        StartCoroutine("IPressUse");
    }
    
    IEnumerator IPressUse()
    {
        useB = true;
        yield return new WaitForSeconds(Time.deltaTime);
        useB = false;

    }
    private void PressInv()
    {
        StartCoroutine("IPressInv");
    }

    IEnumerator IPressInv()
    {
        invB = true;
        yield return new WaitForSeconds(Time.deltaTime);
        invB = false;

    }

}
