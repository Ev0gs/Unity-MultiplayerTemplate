using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    public class NetworkPlayerController : NetworkBehaviour
    {
        private float moveSpeed;
        private Vector2 _movement;
        private Rigidbody2D _rb;
        public TMP_Text playerName;

        /*private NetworkVariable<int> _variable = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public override void OnNetworkSpawn()
        {
            _variable.OnValueChanged += (int previousValue, int nextValue) =>
            {
                Debug.Log(_variable.Value);
            };
        }*/

        private void Awake()
        {
            moveSpeed = 3f;

            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        { 
            if (!IsOwner) return;

            /*if(Input.GetKeyDown(KeyCode.T))
            {
                _variable.Value += 10;
            }*/
            // Initialize movement
            _movement.x = Input.GetAxisRaw("Horizontal");
            _movement.y = Input.GetAxisRaw("Vertical");

        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;

            // Move the player
            _rb.MovePosition(_rb.position + _movement * moveSpeed * Time.fixedDeltaTime);
        }
    }
}