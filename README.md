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

![Decorator](https://github.com/PingAK9/ShootEditor/blob/master/Image/gundecorator.png)


## 3. Strategy pattern
- Decorator pattern (mẫu chiến lược): hiểu một cách đơn giản thì đây là mẫu thiết kế giúp bạn trừu tượng hóa những hành vi (behavior, method, function) của một đối tượng bằng cách đưa ra những cài đặt vào những lớp khác nhau.
### Ví dụ
-  Tạo ra các đối tượng shot của boss, ở mỗi state, mỗi %máu của boss nó sẽ thực hiện các hành động khác nhau. 
1. Đầu tiên, bạn tạo 1 giao diện interface cho phương thức shot
``` csharp
public interface IShot
{
    IEnumerator Shot(Mover mover);
}
```
2. Sau đó bạn sẽ tạo các lớp cụ thể cho từng thuật toán.
``` csharp
public class Shot_Aiming : IShot
{
    float speed = 0.02f;
    public IEnumerator Shot(Mover mover)
    {
        Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
        b.Init(BulletName.red, mover._X, mover._Y, GetPlayerAngle(mover._X, mover._Y), speed);
        yield return null;
    }

    float GetPlayerAngle(float x, float y)
    {
        Vector2 playerPos = GameSystem._Instance.player._pos;
        return Mathf.Atan2(playerPos.y - y, playerPos.x - x) / Mathf.PI / 2.0f;
    }
}

public class Shot_Circle : IShot
{
    float angle = 0.0f;
    float speed = 0.02f;
    int count = 12;
    bool halfAngleOffset = true;
    public IEnumerator Shot(Mover mover)
    {
        float angleStart = angle + ((halfAngleOffset) ? (1.0f / count / 2.0f) : 0.0f);
        for (int i = 0; i < count; ++i)
        {
            Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(BulletName.blue, mover._X, mover._Y, angleStart + (1.0f / count * i), speed);
        }
        yield return null;
    }
}
```
-> Mục đích là tách được các phương thức xử lý ra khỏi các lớp cụ thể như Boss, MiniBoss hay Enemy. Bây giờ bạn đã có thể đưa các thuật toán này vào sử dụng được rồi đấy.
3. Sử dụng thuật toán
``` csharp
public class Boss : Enemy
{
    private IEnumerator MoveMain()
    {
        StartCoroutine(_Logic.MoveConstantVelocity(this, new Vector2(0.0f, 0.75f), 30));
        yield return new WaitForFrames(100);
        //
        yield return StartCoroutine(new Shot_Aiming().Shot(this));
        yield return StartCoroutine(new Shot_Circle().Shot(this));
    }
}
```
## 4. Factory pattern
- Factory pattern được sử dụng để có thể tạo ra nhiều đối tượng khác nhau từ nhiều class
### Ví dụ
- Ở ví dụ của stagery pattern, chúng ta đã tạo ra các hành vi khác nhau của boss, giờ chúng ta sử dụng Factory pattern để tạo ra iShot mà mình mong muôn 
- Mục đích làm cho việc tao 1 object 1 cách dễ dàng, che dấu xử lý logic của việc khởi tạo, Giảm sự phụ thuộc (sau này dù bạn có thêm nhiều đối tượng nữa thì cũng ko cần sửa gì thêm nhiều). 
1. 
``` csharp
class ShotFactory
{
    public ShotFactory()
    {

    }

    public IShot CreateShot(String name)
    {
        switch (name)
        {
            case shotFormBorder:
                return new Shot_FormBorder();
            case shotFormCorner:
                return new Shot_FormCorner();
            case circle:
                return new Shot_Circle();
            default:
                return null
        }
    }
}
```
2. Sử dụng
``` csharp
Queue<IShot> stateStack = new Queue<IShot>();
ShotFactory shotFactory = new ShotFactory();
void AddShotBuilder(String name)
{
    IShot shot = shotFactory.CreateShot(name);
    if (shot != null)
    {
        stateStack.Enqueue(shot);
    }
}
```
## 5. Object Pool pattern
- Chúng ta dùng Object Pool Pattern quản lý một tập hợp các objects mà sẽ được tái sử dụng trong chương trình. Chúng được gọi ra từ pool, sử dụng trong một khoảng thời gian nhất định rồi trả về pool. Trong khoảng thời gian vắng mặt đó của object, không thành phần nào có thể sử dụng tận khi nó được quay trở về pool.
### Ví dụ
- Các đội tượng kế từ từ class mover: Bullet, Player, Enemy, Boss
 -> ShapePool.cs
- Các Sprite render để vẽ các đối tượng trên 
 -> MoverPool.cs
## 6. Template pattern
- Template pattern method định nghĩa một bộ khung của một thuật toán trong một chức năng, chuyển giao việc thực hiện nó cho các lớp con. Mẫu Template Method cho phép lớp con định nghĩa lại cách thực hiện của một thuật toán, mà không phải thay đổi cấu trúc thuật toán
### Ví dụ
- Tạo ra gamelogic và boss khác nhau cho mỗi round.
``` csharp
public abstract partial class BaseGameLogic
{
    abstract void LoadContext();

    abstract IEnumerator UpdateGamePlay();
    
    abstract void GameOver();
}
```
- Ta tạo ra temlate cho class GameLogic, với các hàm LoadContext, Update, và callback GameOver.
- Giờ chúng ta tuỳ chỉnh các hàm này tuỳ theo mỗi round, Ví dụ Round1
``` csharp
public class GameLogic_Round1 : BaseGameLogic
{
    void LoadContext()
    {
        //LoadData Round1
        // Init boss1
    }

    IEnumerator UpdateGamePlay()
    {
        // Update round 1   
    }
    ///
    ...
}
```

## 7. State pattern
- State Pattern là cách tổ chức quản lý các State một cách hợp lý nhất. Giảm bớt rắc rối trong quá trình xử lý các sự kiện. Dễ dàng chỉnh sửa trong quá trình lập trình.
### Ví dụ
- Qua ví dụ trước (Template pattern) chúng ta đã tạo ra Template GameLogic, giờ sẽ áp dụng State pattern để sử dụng nó.
1. Đầu tiên, các bạn phải xác định được các State của đối tượng và tạo Enum cho các State đó.
``` csharp
enum StateType
{
    Invalid,
    Load,
    Play,
    over,
}
```
2. Áp dụng vào trong class đối tượng GameSystem.cs 
``` csharp
partial class GameSystem : MonoBehaviour
{
    private StateType stateType = StateType.Invalid;
    private void SetState(StateType nextState)
    {
        stateType = nextState;
        switch (stateType)
        {
            case StateType.Load:
                logic.LoadContext();
                break;
            case StateType.Play:
                StartCoroutine(logic.UpdateGamePlay());
                break;
            case StateType.Over:
                logic.GameOver();
                break;
        }
    }
}
```
## 8. Builder pattern
- Khi một object có nhiều parameter, thì việc tạo một object có nhiều parameter hoặc nhiều constructor sẽ làm cho code chúng ta trông khá là rườm rà và dễ gây lỗi (chúng ta sẽ vất vả nhưng hiệu quả không cao). Builder là một thiết kế giúp việc tạo một object phức tạp bằng nhiều object đơn giản.

## 9. Observer pattern
- Observer pattern là một mẫu thiết kế phần mềm mà một đối tượng, gọi là subject, duy trì một danh sách các thành phần phụ thuộc nó, gọi là observer, và thông báo tới chúng một cách tự động về bất cứ thay đổi nào, thường thì bằng cách gọi 1 hàm của chúng.
### Ví dụ
- Điều kiện để thua 1 round là khi player chết, còn khi thắng 1 round là khi boss chết. 
- Ví dụ áp dụng Observer pattern vào kiểm tra round thua. khi đó player là subject, gameSystem là observer
``` csharp
partial class Player : Mover
{
    private void OnDead()
    {
        GameSystem.GetInstance.PlayerDead();
    }
}
```
## 10. Composite Pattern
- Tổ chức các đối tượng theo cấu trúc phân cấp dạng cây. Tất cả các đối tượng trong cấu trúc được thao tác theo một cách thuần nhất như nhau.
