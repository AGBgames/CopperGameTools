namespace CopperGameTools.Builder;

public class CGTProjBuilder
{
    public CGTProjFile ProjFile { get; }

    public CGTProjBuilder(CGTProjFile cgtProjFile)
    {
        ProjFile = cgtProjFile;
    }

    public CGTProjBuilderResultType Build()
    {
        foreach (var error in ProjFile.FileCheck().ResultErrors)
            if (error.IsCritical)
                return CGTProjBuilderResultType.FailedWithErrs;

        var keys = ProjFile.KeysToList();

        var projNameKey = ProjFile.KeyFromList("projectname", keys);


        return CGTProjBuilderResultType.DoneNoErrs;
    }
}

public enum CGTProjBuilderResultType
{
    DoneNoErrs,
    DoneWithErrs,
    FailedNoErrs,
    FailedWithErrs
}