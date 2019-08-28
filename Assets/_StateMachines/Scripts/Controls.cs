using UnityEngine;

public class Controls : MonoBehaviour
{
    #region Keyboard & Mouse

    [Header(" Keyboard and Mouse")]

    public KeyCode kPause;
    public KeyCode kDash;
    public KeyCode kJump;
    public KeyCode kHeal;
    public KeyCode kAttack;
    public KeyCode kInteract;
    public KeyCode kShootSkill;
    public KeyCode kDefenceSkill;
    public KeyCode kUltimateMode;
    public KeyCode kInventoryAndMap;

    public string kHorizontal;
    public string kVertical;

    #endregion


    //[Header(" Joystick ")]

    //public KeyCode jPause;
    //public KeyCode jDash;
    //public KeyCode jJump;
    //public KeyCode jHeal;
    //public KeyCode jAttack;
    //public KeyCode jDefense;
    //public KeyCode jInteract;
    //public KeyCode jUltimateMode;
    //public KeyCode jInventoryAndMap;

    public static Controls Instance;
    
    private  void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;


    }
}
