using Kuchen;
using UnityEngine;

namespace Game
{
    public interface IClickable
    {
        void OnPointerDown();
        void OnHold();
        void OnClick();
        void OnPointerUp();
    }

    public class PlayerInputManager : MonoBehaviour
    {
        [Header("Hold Config")]
        [SerializeField] private float holdThreshold = 0.4f;

        private bool isHolding;
        private bool hasHoldTriggered;
        private float holdTimer;

        private RaycastHit currentHit;
        private bool hasHit;

        private Camera mainCam;

        private void Awake()
        {
            mainCam = Camera.main;
        }

        private void Update()
        {
            // INPUT DOWN
            if (TryGetInputDown(out Vector2 pos))
            {
                HandleInputDown(pos);
            }

            // HOLD
            if (isHolding && hasHit && !hasHoldTriggered)
            {
                holdTimer += Time.deltaTime;

                if (holdTimer >= holdThreshold)
                {
                    TriggerHold();
                }
            }

            // INPUT UP
            if (TryGetInputUp())
            {
                HandleInputUp();
            }
        }

        #region Input Handle

        private void HandleInputDown(Vector2 screenPos)
        {
            isHolding = true;
            hasHoldTriggered = false;
            holdTimer = 0f;
            hasHit = false;

            this.Publish(GameEvent.OnPointerDown);
            //Ray ray = mainCam.ScreenPointToRay(screenPos);
            //int ignoreMask = ~LayerMask.GetMask("IgnoreCollider");

            //if (Physics.Raycast(ray, out currentHit, 1000f, ignoreMask))
            //{
            //    hasHit = true;

            //    if (currentHit.collider.TryGetComponent<IClickable>(out var clickable))
            //    {
            //        clickable.OnPointerDown();
            //    }
            //}

            //Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 1f);
        }

        private void TriggerHold()
        {
            hasHoldTriggered = true;

            //if (currentHit.collider.TryGetComponent<IClickable>(out var clickable))
            //{
            //    clickable.OnHold();
            //}
            this.Publish(GameEvent.OnHolded);
        }

        private void HandleInputUp()
        {
            //if (hasHit)
            //{
                if (!hasHoldTriggered)
                {
                    //if (currentHit.collider.TryGetComponent<IClickable>(out var clickable))
                    //{
                    //    clickable.OnClick();
                    //}
                    this.Publish(GameEvent.OnCliked);
                }

                //if (currentHit.collider.TryGetComponent<IClickable>(out var up))
                //{
                //    up.OnPointerUp();
                //}
                this.Publish(GameEvent.OnPointerUp);
            //}

            ResetInput();
        }

        private void ResetInput()
        {
            isHolding = false;
            hasHoldTriggered = false;
            hasHit = false;
            holdTimer = 0f;
        }

        #endregion

        #region Platform Input

        private bool TryGetInputDown(out Vector2 pos)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0))
            {
                pos = Input.mousePosition;
                return true;
            }
#elif UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0 &&
                Input.GetTouch(0).phase == TouchPhase.Began)
            {
                pos = Input.GetTouch(0).position;
                return true;
            }
#endif
            pos = default;
            return false;
        }

        private bool TryGetInputUp()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return Input.GetMouseButtonUp(0);
#elif UNITY_ANDROID || UNITY_IOS
            return Input.touchCount > 0 &&
                   Input.GetTouch(0).phase == TouchPhase.Ended;
#else
            return false;
#endif
        }

        #endregion
    }
}
