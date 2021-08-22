using UnityEngine;

namespace Gameplay.Enemy
{
    public abstract class AbstractEnemy : Character
    {
        public Player player;
        public float minPlayerDistance = 5f;
        public Transform[] path;
        public float minTargetDistance = 0.02f;
        public float waitTime = 1f;

        protected int PositionId;

        private float _waitTime;

        protected bool IsNextToPlayer()
        {
            var playerPosition = player.transform.position;
            var currentPosition = transform.position;

            return Vector2.Distance(currentPosition, playerPosition) < minPlayerDistance;
        }

        protected bool IsNotOnTargetPosition()
            => !IsOnTargetPosition();

        protected bool IsOnTargetPosition()
            => Vector2.Distance(transform.position, TargetPosition) < minTargetDistance;

        protected void ChoosePositionId()
            => PositionId = Random.Range(0, path.Length);

        protected void ResetWaitTime()
            => _waitTime = waitTime;

        protected void Wait()
        {
            if (IsTimeUp())
                ResetWaitTime();
            else
                _waitTime -= Time.deltaTime;
        }

        protected bool IsTimeUp()
            => _waitTime <= 0;
    }
}