namespace CopperGameTools.Builder;

public class CGTProjBuilder
{
    public CGTProjFile ProjFile { get; }


    public CGTProjBuilder(CGTProjFile cgtProjFile)
    {
        ProjFile = cgtProjFile;
    }

    public CGTProjBuilderResult Build()
    {
        foreach (var error in ProjFile.FileCheck().ResultErrors)
            if (error.IsCritical)
                return new CGTProjBuilderResult(CGTProjBuilderResultType.FailedWithErrors);

        return new CGTProjBuilderResult(CGTProjBuilderResultType.DoneNoErrors);
    }
}

public class CGTProjBuilderResult
{
    public CGTProjBuilderResult(CGTProjBuilderResultType cgtProjBuilderResultType)
    {
        ResultType = cgtProjBuilderResultType;
    }

    public CGTProjBuilderResultType ResultType { get; }
}

public enum CGTProjBuilderResultType
{
    DoneNoErrors,
    DoneWithErrors,
    FailedNoErrors,
    FailedWithErrors
}
