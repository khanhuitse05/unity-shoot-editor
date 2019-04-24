# Shot Editor
Mô tả project qua OOP và design pattern
# OOP (Hướng đối tượng)

# Design pattern
Mô tả project qua design pattern
## 1. Singleton pattern
- Singleton pattern cho phép bạn đảm bảo rằng một lớp chỉ có một thể hiện, và có thể truy cập dễ dàng.
### Ví dụ
- Class CoroutineManager -> Quản lý và update các IEnumerator
```csharp
    public class CoroutineManager
    {
        private static CoroutineManager _instance = new CoroutineManager();
        public static CoroutineManager instance {get{return _instance;}}
        public CoroutineManager()
        {
        }
     }
```
Lưu ý: với cách này thì nếu class này chưa được hoặc không sử dụng thì object cũng đã được khởi tạo. sử dụng khi đối tượng có khả nang cao sẽ được sử dụng
- Các trường hợp khác: GameSystem.cs, ..
## 2. Decorator pattern
- Decorator pattern
### Ví dụ

## 3. Factory pattern
- Factory pattern
### Ví dụ

## 4. Object Pool pattern
- Object Pool pattern
### Ví dụ

## 5. Decorator pattern
- Decorator pattern
### Ví dụ

## 6. Template pattern
- Decorator pattern
### Ví dụ

## 7. Builder pattern
- Decorator pattern
### Ví dụ

## 8. Observer pattern
- Decorator pattern
### Ví dụ

## 9. State pattern
- Decorator pattern
### Ví dụ

## 10. Strategy pattern
- Decorator pattern
### Ví dụ
