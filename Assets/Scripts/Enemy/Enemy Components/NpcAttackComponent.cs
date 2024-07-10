using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.AI;

namespace Enemies
{
    public class NpcAttackComponent : MonoBehaviour
    {
        [Header("Attack Objects")]
        public Transform baseTransform;
        public AttackEnemiesObject[] attackEnemiesObjectsArr = new AttackEnemiesObject[0];
        public Collider[] colliderAttackArray = new Collider[0];
        public GameObject[] projectileAttackArrray = new GameObject[0];

        private bool[] isAttackOnCooldown = new bool[0];
        private float[] timerAttackCooldown = new float[0];

        private AttackNPCData currentAttackData;
        private int currentAttackIndex;
        private float m_timer;
        private NpcMetaInfos m_npcMetaInfos;
        private NpcMouvementComponent m_mouvementComponent;
        private NPCEnemiAnimation m_NPCEnemiAnimation;
        private NPCAttackFeedbackComponent m_NPCAttackFeedbackComponent;


        private bool isMvtAttackInit = false;
        private Vector3 prepTargetPosition;

        [Header("Attack Infos")]
        public AttackPhase currentAttackState;
        public bool isActiveDebug = false;



        // Event for each attack step
        public Action OnPrepAttack;
        public Action OnContactAttack;
        public Action OnRecoverAttack;
        public Action<bool> OnFinishAttack;

        #region MonoBehavior Functions
        public void Start()
        {
            m_npcMetaInfos = GetComponent<NpcMetaInfos>();
            m_mouvementComponent = GetComponent<NpcMouvementComponent>();
            m_NPCEnemiAnimation = GetComponent<NPCEnemiAnimation>();
            m_NPCAttackFeedbackComponent = GetComponent<NPCAttackFeedbackComponent>();
            currentAttackState = AttackPhase.NONE;
            InitComponent();
            if (m_NPCEnemiAnimation)
            {
                OnPrepAttack += m_NPCEnemiAnimation.CallCloseAnimation;
                OnPrepAttack += m_NPCEnemiAnimation.CallAnimPrepAttack;


                OnRecoverAttack += m_NPCEnemiAnimation.ResetAnimAttack;
                OnRecoverAttack += m_NPCEnemiAnimation.ResetCloseAnimation;
            }


        }

        public void Update()
        {
            UpdatePrepAttack();
            UpdateContactAttack();
            UpdateRecoverAttack();
            UpdateCooldown();
        }

        #endregion

        public void InitComponent()
        {
            // Setuping Attack
            int countCloseAttack = 0;
            int countRangeAttack = 0;
            for (int i = 0; i < attackEnemiesObjectsArr.Length; i++)
            {
                AttackEnemiesObject currObj = attackEnemiesObjectsArr[i];
                if (currObj.data.typeAttack == AttackType.COLLIDER_OBJ)
                {
                    currObj.data.indexCollider = countCloseAttack;
                    countCloseAttack++;
                }
                else
                {
                    currObj.data.indexProjectileGO = countRangeAttack;
                    countRangeAttack++;
                }

            }

            timerAttackCooldown = new float[attackEnemiesObjectsArr.Length];
            isAttackOnCooldown = new bool[attackEnemiesObjectsArr.Length];

        }

        public void CancelAttack()
        {
            if (!currentAttackData.isStunLockingAttack) return;

            OnFinishAttack?.Invoke(false);
            currentAttackState = AttackPhase.NONE;
        }

        #region  Active Phase Functions
        public void ActivePrepationAttack(int index)
        {
            OnPrepAttack?.Invoke();
            currentAttackState = AttackPhase.PREP;
            currentAttackIndex = index;
            currentAttackData = attackEnemiesObjectsArr[index].data;
            m_mouvementComponent.DirectRotateToTarget();
            if (currentAttackData.isStopMovingAtPrep)
            {
                m_mouvementComponent.StopMouvement();
                m_npcMetaInfos.state = NpcState.ATTACK;
            }
            m_timer = 0.0f;


            AttackInfoData attackInfoData = new AttackInfoData();
            attackInfoData.attackIndex = currentAttackIndex;
            attackInfoData.attackNPCData = currentAttackData;
            attackInfoData.duration = currentAttackData.prepationTime;
            attackInfoData.positionAttack = m_mouvementComponent.targetData.baseTarget.position;
            attackInfoData.phase = AttackPhase.PREP;
            m_NPCAttackFeedbackComponent.SpawnFeedbacks(attackInfoData);
            
            prepTargetPosition = m_mouvementComponent.targetData.baseTarget.position;
            isMvtAttackInit = false;

          


            if (isActiveDebug) Debug.Log($"Agent {transform.gameObject.name} is preparing to attack");
        }



        public void ActiveAttackContact()
        {
            OnContactAttack?.Invoke();
            currentAttackState = AttackPhase.CONTACT;

            AttackInfoData attackInfoData = new AttackInfoData();
            attackInfoData.attackIndex = currentAttackIndex;
            attackInfoData.attackNPCData = currentAttackData;
            attackInfoData.duration = currentAttackData.contactTime;
            attackInfoData.positionAttack = prepTargetPosition;
            attackInfoData.phase = AttackPhase.CONTACT;
            m_NPCAttackFeedbackComponent.SpawnFeedbacks(attackInfoData);

           
            if (currentAttackData.launchMoment == AttackLaunchMoment.START_CONTACT)
            {
                LaunchAttack();
            }

            m_timer = 0.0f;
            if (isActiveDebug) Debug.Log($"Agent {transform.gameObject.name} is attacking with {currentAttackData.nameAttack} the target");

        }

        public void ActiveRecoverPhase()
        {
            OnRecoverAttack?.Invoke();
            currentAttackState = AttackPhase.RECOVERY;

            AttackInfoData attackInfoData = new AttackInfoData();
            attackInfoData.attackIndex = currentAttackIndex;
            attackInfoData.attackNPCData = currentAttackData;
            attackInfoData.duration = currentAttackData.recoverTime;
            attackInfoData.positionAttack = prepTargetPosition;
            attackInfoData.phase = AttackPhase.RECOVERY;
            m_NPCAttackFeedbackComponent.SpawnFeedbacks(attackInfoData);
            m_timer = 0.0f;
            if (isActiveDebug) Debug.Log($"Agent {transform.gameObject.name} is recoving from the attack launch");
        }

        #endregion

        #region  Finish Phase Functions
        public void FinishPreparationAttack()
        {
            m_timer = 0.0f;
            ActiveAttackContact();
        }

        public void FinishContactAttack()
        {
            m_timer = 0.0f;
            if (currentAttackData.typeAttack == AttackType.COLLIDER_OBJ) colliderAttackArray[currentAttackData.indexCollider].gameObject.SetActive(false);
            ActiveRecoverPhase();
        }

        public void FinishRecoverAttack()
        {
            isAttackOnCooldown[currentAttackIndex] = true;
            OnFinishAttack?.Invoke(true);
            currentAttackState = AttackPhase.NONE;
            m_timer = 0.0f;
            if (isActiveDebug) Debug.Log($"Agent {transform.gameObject.name} has finished to attack");
        }
        #endregion

        #region Update Phase Functions
        public void UpdatePrepAttack()
        {
            if (currentAttackState != AttackPhase.PREP) return;

            if (currentAttackData.isFollowTarget || m_timer < currentAttackData.rotationTime)
            {
                m_mouvementComponent.DirectRotateToTarget();
            }
            else
            {
                if (currentAttackData.attackMovement && !isMvtAttackInit)
                {
                    isMvtAttackInit = true;

                       NPCMoveAttData nPCMoveAttData = new NPCMoveAttData();
                    nPCMoveAttData.npcGo = this.gameObject;
                    nPCMoveAttData.maxHeight = GetComponent<NavMeshAgent>().height;
                    nPCMoveAttData.targetTransform = m_mouvementComponent.targetData.baseTarget;

                    currentAttackData.attackMovement.StartMvt(nPCMoveAttData);

                    if (currentAttackData.launchMoment == AttackLaunchMoment.AFTER_MVT)
                        currentAttackData.attackMovement.EndMovement += LaunchAttack;
                }
            }

            if (m_timer > currentAttackData.prepationTime)
            {
                FinishPreparationAttack();
            }
            else
            {
                m_timer += Time.deltaTime;
            }
        }

        public void UpdateContactAttack()
        {
            if (currentAttackState != AttackPhase.CONTACT) return;

            if (currentAttackData.attackMovement)
            {
                currentAttackData.attackMovement.UpdateMvt();
            }

            if (m_timer > currentAttackData.contactTime)
            {
                FinishContactAttack();
          
            }
            else
            {

                if (m_timer > currentAttackData.updateTimeAttackLaunch)
                {
                    if (currentAttackData.launchMoment == AttackLaunchMoment.UPDATE_CONTACT)
                    {
                        LaunchAttack();
                    }
                }

                m_timer += Time.deltaTime;
            }

            
        }

        public void UpdateRecoverAttack()
        {
            if (currentAttackState != AttackPhase.RECOVERY) return;

            if (m_timer > currentAttackData.recoverTime)
            {
                FinishRecoverAttack();
            }
            else
            {
                m_timer += Time.deltaTime;
            }
        }
        #endregion

        public void UpdateCooldown()
        {
            for (int i = 0; i < timerAttackCooldown.Length; i++)
            {
                if (!isAttackOnCooldown[i]) continue;

                if(timerAttackCooldown[i]> attackEnemiesObjectsArr[i].data.coooldownAttack)
                {
                    isAttackOnCooldown[i] = false;
                    timerAttackCooldown[i] = 0;
                    Debug.Log("Attack" + attackEnemiesObjectsArr[i].data.nameAttack + "cooldown is finish");
                }
                else
                {
                    timerAttackCooldown[i] += Time.deltaTime;
                }
            }
        }

        #region Attack Data Function

        public float GetAttackRange(int index) { return attackEnemiesObjectsArr[index].data.attackRange; }

        public void LaunchAttack()
        {

            if (currentAttackData.typeAttack == AttackType.COLLIDER_OBJ)
            {
                colliderAttackArray[currentAttackData.indexCollider].gameObject.SetActive(true);
                CloseAttackComponent closeAttackComponent = colliderAttackArray[currentAttackIndex].gameObject.GetComponent<CloseAttackComponent>();
                closeAttackComponent.damage = currentAttackData.damage;
                closeAttackComponent.isHeavyAttack = currentAttackData.isHeavyAttack;
            }
            else
            {
                GameObject projectile = projectileAttackArrray[currentAttackData.indexProjectileGO];
                Vector3 spawnPosition = Vector3.zero;

                if (currentAttackData.rangeTypeAttack == RangeAttackType.PROJECTILE) spawnPosition = baseTransform.position + baseTransform.forward * 1;
                if (currentAttackData.rangeTypeAttack == RangeAttackType.AREA) { spawnPosition = prepTargetPosition; }

                GameObject instance = Instantiate(projectile, spawnPosition, Quaternion.identity);

                if (currentAttackData.rangeTypeAttack == RangeAttackType.PROJECTILE)
                {
                    instance.transform.rotation = baseTransform.rotation;
                    NPCAttackProjectile npcAttackProjectile = instance.GetComponent<NPCAttackProjectile>();
                    npcAttackProjectile.damage = currentAttackData.damage;
                    npcAttackProjectile.direction = transform.forward;
                    npcAttackProjectile.duration = currentAttackData.durationProjectile;
                    npcAttackProjectile.range = currentAttackData.rangeProjectile;


                }


                if (currentAttackData.rangeTypeAttack == RangeAttackType.AREA)
                {
                    NPCAttackArea attackObjectArea = instance.GetComponent<NPCAttackArea>();
                    attackObjectArea.target = m_mouvementComponent.targetData.baseTarget;
                    attackObjectArea.sizeArea = currentAttackData.radius;
                    attackObjectArea.type = currentAttackData.shapeType;
                    attackObjectArea.damage = currentAttackData.damage;

                }
            }
        }

        public bool IsAttackOnCooldown(int index)
        {
          return  isAttackOnCooldown[index];
        }
        

        #endregion

    }
}