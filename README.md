# Shot Editor
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
- Decorator pattern cho phép người dùng thêm các tính năng mới vào một đối tượng đã có mà không làm thay đổi cấu trúc lớp của nó.
### Ví dụ
- Tuỳ thuộc vào đối tượng nào, hoặc level của súng là bao nhiêu mà số lượng và các loại súng đạn sẽ khác nhau, Thay vì chúng ta tạo ra lớp trừu tượng rồi kế thừa vào override lại hàm shot. Ta sẽ áp dụng decorator pattern
1.  Tạo lớp trừu tượng và lớp trang trí
``` csharp
abstract class Component
{
    public abstract void Operation(Mover mover);
}
class Guns : Component
{
    public override void Operation(Mover mover)
    {
    }
}
abstract class GunDecorator : Component
{
    protected Component component;
    public void SetComponent(Component component)
    {
        this.component = component;
    }

    public override void Operation(Mover mover)
    {
        if (component != null)
        {
            component.Operation(mover);
        }
    }
}
```
2. Tạo ra các đối tượng Decorator: Normal gun, Wing gun và Missiles gun
``` csharp
class NormalGun : GunDecorator
{   
    public override void Operation(Mover mover)
    {
        base.Operation(mover);
        Debug.log("Normal Shot");
    }
}
class WingGun : GunDecorator
{   
    public override void Operation(Mover mover)
    {
        base.Operation(mover);
        Debug.log("Wing Shot");
    }
}
class MissilesGun : GunDecorator
{   
    public override void Operation(Mover mover)
    {
        base.Operation(mover);
        Debug.log("Misslies Shot");
    }
}
```
3. Sử dụng 
``` csharp
public class Player : Mover
{
    GunDecorator gun;
    public override void Init(string shapeSubPath, float x, float y, float angle)
    {
        base.Init(shapeSubPath, x, y, angle);
        // 0
        NormalGun _gun = new NormalGun();
        _gun.SetComponent(new Guns());
        // 1
        NormalGun _gun1 = new NormalGun();
        _gun1.SetComponent(_gun);
        // 2
        MissilesGun _gun2 = new NormalGun();
        _gun2.SetComponent(_gun1);
        // 3
        MissilesGun _gun3 = new NormalGun();
        _gun3.SetComponent(_gun2);
        // 4
        MissilesGun _gun4 = new NormalGun();
        _gun4.SetComponent(_gun3);
        // finally
        gun = _gun4;
    }
        
    private void CreateShot(Mover mover)
    {
        gun.Operation(this);
    }
}
```
+ 4. Sau khi chạy chương trình thì ta sẽ được kết quả như sau
![Screeshot](https://github.com/PingAK9/ShootEditor/Image/master/Images/gundecorator.png)

## 3. Factory pattern
- Factory pattern Nhằm giải quyết vấn đề tạo một đối tượng mà không cần thiết chỉ ra một cách chính xác lớp nào sẽ được tạo
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
