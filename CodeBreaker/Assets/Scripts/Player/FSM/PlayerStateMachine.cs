using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
namespace CodeBreaker
{
    public class PlayerStateMachine : StateMachine<PlayerState>
    {
      

        /// <summary>
        /// Permet d'ajouter un etat dans la machine
        /// </summary>
        /// <param name="state">etat en question</param>
        public override void Add(PlayerState state)
        {
            base.Add(state);
            state.Machine = this;
            state.Player = GetComponent<Player>();
        }
        /// <summary>
        /// permet d'excuter / entrer dans un etat
        /// </summary>
        /// <typeparam name="G">etat quelconque choisi</typeparam>
        public override void Set<G>(bool TriggerEnter = true)
        {
            base.Set<G>(TriggerEnter);
        }

        public void Init()
        {
            Add(new MoveState());
            Add(new AirState());
            Add(new WallState());
            Add(new DashState());
            Initialize<MoveState>();

        }

        public void Update()
        {
            CurrentState?.OnUpdate();
        }

        public void FixedUpdate()
        {
            CurrentState?.OnFixedUpdate();
        }
    }
}