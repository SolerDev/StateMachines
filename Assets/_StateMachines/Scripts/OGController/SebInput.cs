using UnityEngine;
namespace SebCharCtrl
{
    //[RequireComponent(typeof(SebPlayer))]
    public class SebInput : MonoBehaviour
    {
        private SebPlayer player;

        private void Start()
        {
            player = GetComponent<SebPlayer>();
        }

        private void Update()
        {
            Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            player.SetDirectionalInput(directionalInput);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.OnJumpInputDown();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                player.OnJumpInputUp();
            }
        }
    }
}