#if UNITY_EDITOR 
namespace RKode.VERO.Editor {
public abstract class TabBase {
    public abstract string Name { get; }
    protected TabContext _ctx;

    protected TabBase(TabContext ctx) {
        _ctx = ctx;
    }

    public abstract void Draw();
}
}
#endif
