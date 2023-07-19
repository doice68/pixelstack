public interface IExporter<T>
{
    public T Export(SpriteStack spriteStack);
    public void ExportAndSave(SpriteStack spriteStack, string dir);
}