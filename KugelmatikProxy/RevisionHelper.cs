namespace KugelmatikProxy
{
    public static class RevisionHelper
    {
        public static bool CheckRevision(int lastRevision, int revision)
        {
            if (revision < 0 && lastRevision >= 0)
                return true;

            return revision > lastRevision;
        }
    }
}
