using Elsa.Workflows;
using Elsa.Workflows.Activities;

namespace Shaml.Workflows.Elsa.Workflows;

public sealed class CaseWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.DefinitionId = "shaml-case-workflow";

        builder.Root = new Sequence
        {
            Activities =
            {
                new WriteLine("Shaml case workflow started")
            }
        };
    }
}