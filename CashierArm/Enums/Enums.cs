namespace CashierArm.Enums
{
    // Id типов документа (должны соответствовать Id таблицы DocumentType)
    public enum DocumentTypeEn
    {
        Receipt = 1,        //Поступление
        Sale = 2,           //Реализация
        Сancellation = 3    //Списание
    }

    public enum OperationTypeEn
    {
        Receipt = 1,        //Приход
        Сost = 2            //Расход
    }
}
