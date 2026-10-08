using Domain.Assignments;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Shaml.Workflows.Elsa.Activities;

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
                new WriteLine(
                    "Shaml case workflow started"),

                new WaitForAssignmentCompleted
                {
                    TaskCode =
                        new(AssignmentTaskCodes.ReviewCase)
                },

                new WriteLine("Specialist case review completed"),

                new CreateDirectorApprovalAssignment(),

                new WriteLine("Center Director approval assignment created"),

                new WaitForAssignmentCompleted
                {
                    TaskCode =
                        new(AssignmentTaskCodes.DirectorApproval)
                },

                new WriteLine("Center Director approval completed"),

                new WriteLine("Shaml case workflow completed")
            }
        };
    }
}