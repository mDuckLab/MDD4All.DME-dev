namespace MDD4All.DME.ViewModels.EditorViewModels.Accesses
{
    public class IndexedAccess : Access
    {
        public IndexedAccess(int index) 
        { 
            this.Index = index;
        }

        public int Index { get; set; }
    }
}
