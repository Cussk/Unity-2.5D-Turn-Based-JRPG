using System;
using UnityEngine;

namespace Characters.Party
{
    public class MemberFollowAI : MonoBehaviour
    {
        static readonly int IsWalking = Animator.StringToHash(GlobalVariables.IS_WALK_PARAM);
        
        Transform _followTarget;
        Animator _animator;
        SpriteRenderer _spriteRenderer;
        int _followDistance;
        readonly int _speed = 5;

        public void Init(Transform followTarget, int followDistance)
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _followTarget = followTarget;
            _followDistance = followDistance;
        }

        void FixedUpdate()
        {
            MoveToPlayer();
        }

        void MoveToPlayer()
        {
            if (_followTarget == null) return;
            
            var followerPosition = transform.position;
            var followTargetPosition = _followTarget.position;
            
            if (Vector3.Distance(followerPosition, followTargetPosition) > _followDistance)
            {
                _animator.SetBool(IsWalking, true);
                var distanceDelta = _speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(followerPosition, followTargetPosition, distanceDelta);

                _spriteRenderer.flipX = followTargetPosition.x - followerPosition.x < 0;
            }
            else
            {
                _animator.SetBool(IsWalking, false); 
            }
        }
    }
}
