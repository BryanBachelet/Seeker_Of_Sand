using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
namespace GuerhoubaGames.VFX
{

    public class VFX_CircleArea : MonoBehaviour
    {
        
        private ObjectState state;
        private VFXAttackMeta vfxMetaComponent;
        private VisualEffect visualEffect;
        private VfxAttackData data;


        // Start is called before the first frame update
        void Start()
        {
           
            InitComponent(); 
        }

        public void OnEnable()
        {
            InitComponent();
        }

        public void InitComponent()
        {
            if (state == null)
            {
                state = new ObjectState();
                GameState.AddObject(state);
            }

            vfxMetaComponent = GetComponent<VFXAttackMeta>();
            visualEffect = GetComponent<VisualEffect>();
            data = vfxMetaComponent.vfxData;

            visualEffect.SetFloat("Size", data.attackRange);
            visualEffect.SetFloat("TempsRealese", data.duration);
            visualEffect.SendEvent("ActiveArea");


            DestroyAfterBasic destroyAfterBasic = gameObject.GetComponent<DestroyAfterBasic>();
            if(destroyAfterBasic ==null) destroyAfterBasic = gameObject.AddComponent<DestroyAfterBasic>();
            destroyAfterBasic.m_DestroyAfterTime = data.duration;
            destroyAfterBasic.isNotDestroying = !data.isDestroying;
            destroyAfterBasic.LaunchTimer();

            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(transform.position + Vector3.up * 3, -Vector3.up, out hit, 100, GameLayer.instance.groundLayerMask))
            {
                float angle = Vector3.SignedAngle(Vector3.up, hit.normal, Vector3.forward);

                transform.rotation = Quaternion.Euler(angle, transform.eulerAngles.y, transform.eulerAngles.z);
                transform.position = hit.point;
            }
        }

    }

}
