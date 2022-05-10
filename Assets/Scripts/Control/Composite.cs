using System.Collections.Generic;

namespace Game.Control.BehaviourTree
{
    public abstract class Composite : Node
    {
        protected List<Node> children = new List<Node>();
        protected Composite(string name = "Composite") : base(name) { }
        public void AddChild(Node node) => children.Add(node);
    }
    /// <summary>
    /// 选择结点
    /// 子节点返回成功:退出
    /// 子节点返回失败:执行下一个子节点
    /// </summary>
    public class Selector : Composite
    {
        public Selector(string name = "Selector") : base(name) { }
        public override Status Execute()
        {
            switch (children[index].Execute())
            {
                case Status.SUCCESS:
                    index = 0;
                    return Status.SUCCESS;
                case Status.RUNNING:
                    break;
                case Status.FAILURE:
                    index++;
                    if (index >= children.Count)
                    {
                        index = 0;
                        return Status.FAILURE;
                    }
                    break;
            }
            return Status.RUNNING;
        }
    }

    public class PrioritySelector : Composite
    {
        bool ordered = false;
        public double priority = 0;
        public PrioritySelector(double priority, string name = "PrioritySelector") : base(name) => this.priority = priority;
        public override Status Execute()
        {
            if (!ordered)
            {
                children.Sort((a, b) =>
                {
                    if ((a as PrioritySelector).priority < (b as PrioritySelector).priority)
                        return -1;
                    else if ((a as PrioritySelector).priority > (b as PrioritySelector).priority)
                        return 1;
                    return 0;
                });
                ordered = true;
            }
            switch (children[index].Execute())
            {
                case Status.SUCCESS:
                    index = 0;
                    ordered = false;
                    return Status.SUCCESS;
                case Status.RUNNING:
                    break;
                case Status.FAILURE:
                    index++;
                    if (index >= children.Count)
                    {
                        index = 0;
                        ordered = false;
                        return Status.FAILURE;
                    }
                    break;
            }
            return Status.RUNNING;
        }
    }

    public class RandomSelector : Composite
    {
        bool shuffled = false;
        public RandomSelector(string name = "RandomSelector") : base(name) { }
        public override Status Execute()
        {
            if (!shuffled)
            {
                children.Shuffle();
                shuffled = true;
            }
            switch (children[index].Execute())
            {
                case Status.SUCCESS:
                    index = 0;
                    shuffled = false;
                    return Status.SUCCESS;
                case Status.RUNNING:
                    break;
                case Status.FAILURE:
                    index++;
                    if (index >= children.Count)
                    {
                        index = 0;
                        shuffled = false;
                        return Status.FAILURE;
                    }
                    break;
            }
            return Status.RUNNING;
        }
    }

    public class Sequence : Composite
    {
        public Sequence(string name = "Sequence") : base(name) { }
        public override Status Execute()
        {
            switch (children[index].Execute())
            {
                case Status.SUCCESS:
                    index++;
                    if (index >= children.Count)
                    {
                        index = 0;
                        return Status.SUCCESS;
                    }
                    break;
                case Status.RUNNING:
                    break;
                case Status.FAILURE:
                    index = 0;
                    return Status.FAILURE;
            }
            return Status.RUNNING;
        }
    }
    /// <summary>
    /// 并行结点
    /// 所有子节点都返回成功则退出
    /// 某个子节点返回失败则返回失败
    /// </summary>
    public class Parallel : Composite
    {
        public Parallel(string name = "Parallel") : base(name) { }
        public override Status Execute()
        {
            int succ_count = 0;
            for (int i = 0; i < children.Count; i++)
            {
                switch (children[i].Execute())
                {
                    case Status.SUCCESS:
                        succ_count++;
                        if (succ_count == children.Count)
                            return Status.SUCCESS;
                        break;
                    case Status.RUNNING:
                        break;
                    case Status.FAILURE:
                        return Status.FAILURE;
                }
            }
            return Status.RUNNING;
        }
    }
}