namespace PlayerState
{
    public class State<T>
    {
        public virtual void Enter(T entity) { }
        public virtual void Execute(T entity) { }
        public virtual void Exit(T entity) { }
    }

    public class Moving : State<PlayerObject>
    {
        private Moving() { }
        private static Moving instance;
        public static Moving Instance
        {
            get
            {
                if (instance == null) instance = new Moving();
                return instance;
            }
        }
        public override void Enter(PlayerObject entity)
        {
            entity.sound.PlayDrivingAudio();
            entity.PrePos = entity.Pos;
        }

        public override void Execute(PlayerObject entity)
        {
            if (entity.currentHp <= 0)
            {
                entity.FSM.ChangeState(Death.Instance);
                return;
            }
            if (entity.PrePos == entity.Pos) entity.FSM.ChangeState(Idling.Instance);
            entity.PrePos = entity.Pos;
        }
    }

    public class Idling : State<PlayerObject>
    {
        private Idling() { }
        private static Idling instance;
        public static Idling Instance
        {
            get
            {
                if (instance == null) instance = new Idling();
                return instance;
            }
        }
        public override void Enter(PlayerObject entity)
        {
            entity.sound.PlayIdlingAudio();
            entity.PrePos = entity.Pos;
        }

        public override void Execute(PlayerObject entity)
        {
            if (entity.currentHp <= 0)
            {
                entity.FSM.ChangeState(Death.Instance);
                return;
            }
            if (entity.PrePos != entity.Pos) entity.FSM.ChangeState(Moving.Instance);
            entity.PrePos = entity.Pos;
        }
    }

    public class Death : State<PlayerObject>
    {
        private Death() { }
        private static Death instance;
        public static Death Instance
        {
            get
            {
                if (instance == null) instance = new Death();
                return instance;
            }
        }

        public override void Enter(PlayerObject entity)
        {
            entity.sound.Mute();
            entity.view.Explode();
        }

        public override void Execute(PlayerObject entity)
        {
            if (entity.currentHp > 0) entity.FSM.ChangeState(Idling.Instance);
        }

        public override void Exit(PlayerObject entity)
        {
            entity.view.Reset();
        }
    }
}
