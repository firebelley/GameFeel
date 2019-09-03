namespace GameFeel.Component.Subcomponent.Behavior
{
    /// <summary>
    /// Evaluates the left node, if true then continues with the right
    /// This will process even if a descendant node is running currently.
    /// This class can be used to abort a behavior if a condition is met
    /// </summary>
    public class RunWhileComposite : BehaviorNode
    {
        private int _processingIndex = 0;

        protected override void InternalEnter()
        {
            _processingIndex = 0;
            SetProcess(true);
        }

        protected override void Tick()
        {
            _processingIndex = 0;
            if (_children.Count > 0)
            {
                _children[0].Enter();
            }
        }

        protected override void ChildStatusUpdated(Status status)
        {
            if (status == Status.FAIL && _processingIndex == 0)
            {
                EmitSignal(nameof(Aborted));
                Leave(Status.FAIL);
            }
            else if (status == Status.FAIL)
            {
                Leave(Status.FAIL);
            }
            else if (status == Status.SUCCESS)
            {
                _processingIndex++;
                if (_processingIndex < _children.Count)
                {
                    if (!_children[_processingIndex].IsRunning)
                    {
                        _children[_processingIndex].Enter();
                    }
                }
                else
                {
                    Leave(Status.SUCCESS);
                }
            }
        }
    }
}