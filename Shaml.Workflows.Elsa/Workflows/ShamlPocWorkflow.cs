using Elsa.Workflows;
using Elsa.Workflows.Activities;

namespace Shaml.Workflows.Elsa.Workflows;

public sealed class ShamlPocWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.DefinitionId = "shaml-poc";

        builder.Root = new Sequence
        {
            Activities =
            {
                new WriteLine("Shaml Elsa workflow started"),
                new WriteLine("Shaml Elsa workflow completed")
            }
        };
    }
}