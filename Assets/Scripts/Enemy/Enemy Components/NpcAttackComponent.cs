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

        public bool isGeneralAttackCooldownActive;
        public float baseCooldownAttack = 0;
        private float baseCooldownAttackTimer;
        private bool m_hasToActiveGeneralCooldown;
        private const int m_generalCooldownFrameCount = 2;
        private int m_generalCooldownFrameCounter;

        [HideInInspector]
        [Range(0, 1f)]
        [Tooltip("Stat of cooldown reduction for all attack")]
        public float attackCooldownReduction = 0.0f;  

        [Header("Attack Infos")]
        public AttackPhase currentAttackState;
        public bool isActiveDebug = false;


        // Event for each attack step
        public Action<int> OnPrepAttack;
        public Action<int> OnContactAttack;
        public Action<int> OnRecoverAttack;
        public Action<bool> OnFinishAttack;

        public List<Action<int>> list_OnPrepAttack = new List<Action<int>>();
        public List<Action> list_OnContactAttack = new List<Action>();
        public List<Action> list_OnRecoverAttack = new List<Action>();
        public List<Action<bool>> list_OnFinishAttack = new List<Action<bool>>();

        public bool variantePrecision = false;
        public float rangeVariante = 1;

        public bool isInAttackSequence;
        public int sequenceIndex =-1;


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
                //for(int i = 0; i < attackEnemiesObjectsArr.Length; i++)
                //{
                //    list_OnPrepAttack[i] += m_NPCEnemiAnimation.CallCloseAnimation;
                //    list_OnPrepAttack[i] += m_NPCEnemiAnimation.CallAnimPrepAttack;
                //
                //    list_OnRecoverAttack[i] += m_NPCEnemiAnimation.ResetAnimAttack;
                //    list_OnRecoverAttack[i] += m_NPCEnemiAnimation.ResetCloseAnimation;
                //}


                OnRecoverAttack += m_NPCEnemiAnimation.ResetAnimAttack;
                OnRecoverAttack += m_NPCEnemiAnimation.ResetCloseAnimation;
            }

        }

        public void Update()
        {
            if (m_hasToActiveGeneralCooldown)
            {
                if (m_generalCooldownFrameCounter > m_generalCooldownFrameCount)
                {
                    isGeneralAttackCooldownActive = true;
                    m_generalCooldownFrameCounter = 0;
                    m_hasToActiveGeneralCooldown = false;
                }
                else
                {
                    m_generalCooldownFrameCounter++;
                }
            }

            if (isGeneralAttackCooldownActive)
            {
                if(baseCooldownAttackTimer >baseCooldownAttack)
                {
                    isGeneralAttackCooldownActive = false;
                    baseCooldownAttackTimer = 0.0f;
                }
                baseCooldownAttackTimer += Time.deltaTime;
                return;
            }
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
                if (currObj.data.customAttack != null) continue;

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
        public void ActivePrepationAttack(int index , int sequence)
        {
            OnPrepAttack?.Invoke(index);
            //list_OnPrepAttack?.Invoke(index);
            currentAttackState = AttackPhase.PREP;
            currentAttackIndex = index;
            currentAttackData = attackEnemiesObjectsArr[index].data;

            if (currentAttackData.isStartinAttackSequence)
            {
                isInAttackSequence = true;
                sequenceIndex = sequence;
            }

            // Custom Attack Section
            if (currentAttackData.customAttack != null)
            {
                currentAttackData.customAttack.customAttackData = FillCustomAttackData(currentAttackData, currentAttackIndex);
                currentAttackData.customAttack.ResetAttack();
                currentAttackData.customAttack.ActivePrepPhase();
                if (currentAttackData.isStopMovingAtPrep)
                {
                    m_mouvementComponent.StopMouvement();
                    m_npcMetaInfos.state = NpcState.ATTACK;
                }

                return;
            }

            //
            m_mouvementComponent.DirectRotateToTarget();
            if (currentAttackData.isStopMovingAtPrep)
            {
                m_mouvementComponent.StopMouvement();
                m_npcMetaInfos.state = NpcState.ATTACK;
            }
            m_timer = 0.0f;

            


            AttackInfoData attackInfoData = new AttackInfoData();
            attackInfoData.attackIndex = currentAttackIndex;
            attackInfoData.radius = currentAttackData.radius;
            attackInfoData.duration = currentAttackData.prepationTime;
           
            attackInfoData.target = m_mouvementComponent.targetData.baseTarget;
            attackInfoData.phase = AttackPhase.PREP;

            if (currentAttackData.postionToSpawnType == AttackNPCData.AttackPosition.Target)
            {
                Vector3 rndPosition = Vector3.zero;
                if (variantePrecision) 
                { 
                    rndPosition = new Vector3(UnityEngine.Random.Range(-rangeVariante, rangeVariante), 0, UnityEngine.Random.Range(-rangeVariante, rangeVariante)); 
                }
                prepTargetPosition = m_mouvementComponent.targetData.baseTarget.position + rndPosition;
            }

            if (currentAttackData.postionToSpawnType == AttackNPCData.AttackPosition.Self) 
                prepTargetPosition = transform.position;

            attackInfoData.positionAttack = prepTargetPosition;
            m_NPCAttackFeedbackComponent.SpawnFeedbacks(attackInfoData);

            isMvtAttackInit = false;


           

            if (isActiveDebug) Debug.Log($"Agent {transform.gameObject.name} is preparing to attack");
        }



        public void ActiveAttackContact()
        {
            OnContactAttack?.Invoke(0);
            currentAttackState = AttackPhase.CONTACT;
            if (isActiveDebug) Debug.Log($"Agent {transform.gameObject.name} is attacking with {currentAttackData.nameAttack} the target");
            // Custom Attack Section
            if (currentAttackData.customAttack != null)
            {
                if (currentAttackData.isStopMovingAtPrep)
                {
                    m_mouvementComponent.StopMouvement();
                    m_npcMetaInfos.state = NpcState.ATTACK;
                }

                currentAttackData.customAttack.ActiveContactPhase();

                return;
            }

            AttackInfoData attackInfoData = new AttackInfoData();
            attackInfoData.attackIndex = currentAttackIndex;
            attackInfoData.radius = currentAttackData.radius;
            attackInfoData.duration = currentAttackData.contactTime;
            attackInfoData.positionAttack = prepTargetPosition;
            attackInfoData.target = m_mouvementComponent.targetData.baseTarget;
            attackInfoData.phase = AttackPhase.CONTACT;
            m_NPCAttackFeedbackComponent.SpawnFeedbacks(attackInfoData);

           
            if (currentAttackData.launchMoment == AttackLaunchMoment.START_CONTACT)
            {
                LaunchAttack();
            }

            m_timer = 0.0f;
            

        }

        public void ActiveRecoverPhase()
        {
            OnRecoverAttack?.Invoke(currentAttackIndex);
            currentAttackState = AttackPhase.RECOVERY;

            // Custom Attack Section
            if (currentAttackData.customAttack != null)
            {
                if (currentAttackData.isStopMovingAtPrep)
                {
                    m_mouvementComponent.StopMouvement();
                    m_npcMetaInfos.state = NpcState.ATTACK;
                }

                currentAttackData.customAttack.ActiveRecoverPhase();

                return;
            }

            AttackInfoData attackInfoData = new AttackInfoData();
            attackInfoData.attackIndex = currentAttackIndex;
            attackInfoData.radius = currentAttackData.radius;
            attackInfoData.duration = currentAttackData.recoverTime;
            attackInfoData.positionAttack = prepTargetPosition;
            attackInfoData.target = m_mouvementComponent.targetData.baseTarget;
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
            // Custom Attack Section
            if (currentAttackData.customAttack != null)
            {
                currentAttackData.customAttack.EndPrepPhase();
            }
            ActiveAttackContact();
        }

        public void FinishContactAttack()
        {
            m_timer = 0.0f;
            // Custom Attack Section
            if (currentAttackData.customAttack != null)
            {
                currentAttackData.customAttack.EndContactPhase();
            }
            else
            {

                if (currentAttackData.typeAttack == AttackType.COLLIDER_OBJ) 
                    colliderAttackArray[currentAttackData.indexCollider].gameObject.SetActive(false);
            }
            ActiveRecoverPhase();
        }

        public void FinishRecoverAttack()
        {
            if (currentAttackData.isEndingAttackSequence)
            {
                    isInAttackSequence = false;
                sequenceIndex = -1;
            }
            if (currentAttackData.isEndingAttackSequence) m_hasToActiveGeneralCooldown = true;

            // Custom Attack Section
            if (currentAttackData.customAttack != null)
            {
                currentAttackData.customAttack.EndRecoverPhase();
            }
   
             isAttackOnCooldown[currentAttackIndex] = true;
            OnFinishAttack?.Invoke(true);
            currentAttackState = AttackPhase.NONE;
            m_npcMetaInfos.state = NpcState.IDLE;
            m_timer = 0.0f;
            if (isActiveDebug) Debug.Log($"Agent {transform.gameObject.name} has finished to attack");
        }

        #endregion

        private void SetGeneralCooldown(bool state)
        {
            if (currentAttackData.isTriggerGeneralCooldown) isGeneralAttackCooldownActive = true;
        }

        #region Update Phase Functions
        public void UpdatePrepAttack()
        {
            if (currentAttackState != AttackPhase.PREP) return;

            // Custom Attack Section
            if (currentAttackData.customAttack != null)
            {
               bool result =  currentAttackData.customAttack.UpdatePrepPhase();
                if(result)
                {
                    FinishPreparationAttack();
                }

                return;
            }

            if (currentAttackData.isFollowTarget || m_timer < currentAttackData.rotationTime)
            {
                m_mouvementComponent.DirectRotateToTarget();
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.angularVelocity = Vector3.zero;
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

                    Rigidbody rb = GetComponent<Rigidbody>();
                    rb.angularVelocity = Vector3.zero;
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

            // Custom Attack Section
            if (currentAttackData.customAttack != null)
            {
                bool result = currentAttackData.customAttack.UpdateContactPhase();
                if (result)
                {
                    FinishContactAttack();
                }

                return;
            }

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

            // Custom Attack Section
            if (currentAttackData.customAttack != null)
            {
                bool result = currentAttackData.customAttack.UpdateRecoverPhase();
                if (result)
                {
                    FinishRecoverAttack();
                }

                return;
            }

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

                if(timerAttackCooldown[i]> attackEnemiesObjectsArr[i].data.coooldownAttack * (1.0f- attackCooldownReduction))
                {
                    isAttackOnCooldown[i] = false;
                    timerAttackCooldown[i] = 0;
                   if(isActiveDebug) Debug.Log("Attack" + attackEnemiesObjectsArr[i].data.nameAttack + "cooldown is finish");
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
                CloseAttackComponent closeAttackComponent = colliderAttackArray[currentAttackData.indexCollider].gameObject.GetComponent<CloseAttackComponent>();
                closeAttackComponent.damage = currentAttackData.damage;
                closeAttackComponent.isHeavyAttack = currentAttackData.isHeavyAttack;
                closeAttackComponent.attackName = currentAttackData.nameAttack;
            }
            else
            {
                GameObject projectile = projectileAttackArrray[currentAttackData.indexProjectileGO];
                Vector3 spawnPosition = Vector3.zero;

                if (currentAttackData.rangeTypeAttack == RangeAttackType.PROJECTILE) spawnPosition = baseTransform.position + baseTransform.forward * 1;
                if (currentAttackData.rangeTypeAttack == RangeAttackType.AREA) { spawnPosition = prepTargetPosition; }


                Quaternion rotObj = Quaternion.Euler(0.0f, transform.eulerAngles.y, 0);
                if (currentAttackData.postionToSpawnType == AttackNPCData.AttackPosition.Target) rotObj = Quaternion.identity;

                GameObject instance = Instantiate(projectile, spawnPosition, rotObj);

                if (currentAttackData.rangeTypeAttack == RangeAttackType.PROJECTILE)
                {
                    instance.transform.rotation = baseTransform.rotation;
                    NPCAttackProjectile npcAttackProjectile = instance.GetComponent<NPCAttackProjectile>();
                    npcAttackProjectile.damage = currentAttackData.damage;
                    npcAttackProjectile.direction = transform.forward;
                    npcAttackProjectile.duration = currentAttackData.durationProjectile;
                    npcAttackProjectile.range = currentAttackData.rangeProjectile;
                    npcAttackProjectile.attackName = currentAttackData.nameAttack;


                }


                if (currentAttackData.rangeTypeAttack == RangeAttackType.AREA)
                {
                    NpcAttackMeta attackObjectArea = instance.GetComponent<NpcAttackMeta>();
                    AttackObjMetaData attackObjMetaData = new AttackObjMetaData();
                    attackObjMetaData.target = m_mouvementComponent.targetData.baseTarget;
                    attackObjMetaData.size = currentAttackData.radius;
                    attackObjMetaData.typeArea = currentAttackData.shapeType;
                    attackObjMetaData.damage = currentAttackData.damage;
                    attackObjMetaData.nameAttack = currentAttackData.nameAttack;
                    attackObjMetaData.isOneShoot = true;
                    attackObjectArea.InitAttackObject(attackObjMetaData);

                }
            }


        }

        public bool IsAttackOnCooldown(int index)
        {
          return  isAttackOnCooldown[index];
        }

        public CustomAttackData FillCustomAttackData(AttackNPCData attackNPCData, int attackIndex)
        {
            CustomAttackData customAttackData = new CustomAttackData();
            customAttackData.name = attackNPCData.nameAttack;
            customAttackData.damage = attackNPCData.damage;
            customAttackData.attackIndex = attackIndex;
            customAttackData.npcAttackFeedback = m_NPCAttackFeedbackComponent;
            customAttackData.npcAttacksComp = this;
            customAttackData.targetTransform = m_mouvementComponent.targetData.baseTarget;
            customAttackData.ownTransform = this.transform;

            return customAttackData;
        }



        #endregion

        public bool IsGeneralRecoveringFromAttackActive()
        {
            return isGeneralAttackCooldownActive;
        }

    }
}