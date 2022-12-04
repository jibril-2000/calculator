using FK_CLI;
using System;

namespace Calculator
{
    class Program
    {
        public static int MapWidth = 50;//部屋の大きさ
        public static int MapHeight = 50;//部屋の大きさ

        public static int[,] Map;
        public static int[,] Cost;

        public static int roomMinHeight = 3;//部屋の大きさ縦最小
        public static int roomMaxHeight = 5;//部屋の大きさ縦最大

        public static int roomMinWidth = 3;//部屋の大きさ横最小
        public static int roomMaxWidth = 5;//部屋の大きさ横最大

        public static int RoomCountMin = 25;//部屋の数最小
        public static int RoomCountMax = 30;//部屋の数最大

        public static fk_Model Me = new fk_Model();
        public static fk_Model AttackObj = new fk_Model();
        public static fk_Model goal = new fk_Model();
        public static fk_Model[] model = new fk_Model[0];
        public static fk_Model[] enemy = new fk_Model[0];


        const int wall = 9;
        const int road = 0;
        static bool Flag = true;

        const int meetPointCount = 1;



        static void ResetMapData(fk_AppWindow argWindow)
        {
            Map = new int[MapHeight, MapWidth];
            for (int i = 0; i < MapHeight; i++)
            {
                for (int j = 0; j < MapWidth; j++)
                {
                    Map[i, j] = wall;
                    
                }
            }
            if (model.Length > 0)
            {
                argWindow.Remove(Me);
                argWindow.Remove(goal);
                for (int i = 0; i <= model.Length-1; i++)
                {
                    //Console.WriteLine(i);
                    argWindow.Remove(model[i]);
                }
            }
            if (enemy.Length > 0)
            {
                for (int i = 0; i <= enemy.Length - 1; i++)
                {
                    
                    argWindow.Remove(enemy[i]);
                }
            }
            
        }
        static void CreateSpaceData()
        {
            int roomCount = fk_Math.Rand(RoomCountMin, RoomCountMax);

            int[] meetPointsX = new int[meetPointCount];
            int[] meetPointsY = new int[meetPointCount];
            for (int i = 0; i < meetPointsX.Length; i++)
            {
                meetPointsX[i] = fk_Math.Rand(MapWidth / 4, MapWidth * 3 / 4);
                meetPointsY[i] = fk_Math.Rand(MapHeight / 4, MapHeight * 3 / 4);
                Map[meetPointsY[i], meetPointsX[i]] = road;
            }

            for (int i = 0; i < roomCount; i++)
            {
                int roomHeight = fk_Math.Rand(roomMinHeight, roomMaxHeight);
                int roomWidth = fk_Math.Rand(roomMinWidth, roomMaxWidth);
                int roomPointX = fk_Math.Rand(2, MapWidth - roomMaxWidth - 2);
                int roomPointY = fk_Math.Rand(2, MapWidth - roomMaxWidth - 2);

                int roadStartPointX = fk_Math.Rand(roomPointX, roomPointX + roomWidth);
                int roadStartPointY = fk_Math.Rand(roomPointY, roomPointY + roomHeight);

                bool isRoad = CreateRoomData(roomHeight, roomWidth, roomPointX, roomPointY);

                if (isRoad == false)
                {
                    CreateRoadData(roadStartPointX, roadStartPointY, meetPointsX[fk_Math.Rand(0, 0)], meetPointsY[fk_Math.Rand(0, 0)]);
                }
            }
        }

        static bool CreateRoomData(int roomHeight, int roomWidth, int roomPointX, int roomPointY)
        {
            bool isRoad = false;
            for (int i = 0; i < roomHeight; i++)
            {
                for (int j = 0; j < roomWidth; j++)
                {
                    if (Map[roomPointY + i, roomPointX + j] == road)
                    {
                        isRoad = true;
                    }
                    else
                    {
                        Map[roomPointY + i, roomPointX + j] = road;
                    }
                }
            }
            return isRoad;
        }

        static void CreateRoadData(int roadStartPointX, int roadStartPointY, int meetPointX, int meetPointY)
        {

            bool isRight;
            if (roadStartPointX > meetPointX)
            {
                isRight = true;
            }
            else
            {
                isRight = false;
            }
            bool isUnder;
            if (roadStartPointY > meetPointY)
            {
                isUnder = false;
            }
            else
            {
                isUnder = true;
            }

            if (fk_Math.Rand(0, 2) == 0)
            {

                while (roadStartPointX != meetPointX)
                {

                    Map[roadStartPointY, roadStartPointX] = road;
                    if (isRight == true)
                    {
                        roadStartPointX--;
                    }
                    else
                    {
                        roadStartPointX++;
                    }

                }

                while (roadStartPointY != meetPointY)
                {

                    Map[roadStartPointY, roadStartPointX] = road;
                    if (isUnder == true)
                    {
                        roadStartPointY++;
                    }
                    else
                    {
                        roadStartPointY--;
                    }
                }

            }
            else
            {

                while (roadStartPointY != meetPointY)
                {

                    Map[roadStartPointY, roadStartPointX] = road;
                    if (isUnder == true)
                    {
                        roadStartPointY++;
                    }
                    else
                    {
                        roadStartPointY--;
                    }
                }

                while (roadStartPointX != meetPointX)
                {

                    Map[roadStartPointY, roadStartPointX] = road;
                    if (isRight == true)
                    {
                        roadStartPointX--;
                    }
                    else
                    {
                        roadStartPointX++;
                    }

                }

            }
        }
        static void CreateDangeon(fk_AppWindow argWindow)
        {
            var wallCount = 0;
            var Num = 0;
            for (int i = 0; i < MapHeight; i++)
            {
                for (int j = 0; j < MapWidth; j++)
                {
                    if (Map[i, j] == wall)
                    {
                        wallCount += 1;
                    }
                }
            }

            model = new fk_Model[wallCount];

            for (int i = 0; i < MapHeight; i++)
            {
                for (int j = 0; j < MapWidth; j++)
                {
                    if (Map[i, j] == wall)
                    {
                        model[Num] = new fk_Model();
                        model[Num].Shape = new fk_Block(1.0, 1.0, 1.0);
                        model[Num].Material = fk_Material.Yellow;
                        model[Num].BMode = fk_BoundaryMode.SPHERE;
                        model[Num].AdjustSphere();
                        model[Num].GlTranslate(new fk_Vector(j - MapWidth / 2, 1, i - MapHeight / 2));
                        argWindow.Entry(model[Num]);
                        Num += 1;

                    }

                }
            }
            argWindow.CameraPos = new fk_Vector(model[Num - 1].Position.x, model[Num - 1].Position.y + 10.0, model[Num - 1].Position.z);
            argWindow.CameraFocus = new fk_Vector(0.0, 0.0, 0.0);

        }
        static void Mecreate(fk_AppWindow argWindow)
        {
            int count1 = 0;
            int mecom = 0;
            for (int i = 0; i < MapHeight; i++)
            {
                for (int j = 0; j < MapWidth; j++)
                {

                    if (Map[i, j] == road)
                    {
                        if (count1 >= fk_Math.Rand(10, 1000) && mecom < 1)
                        {
                            Me = new fk_Model();
                            Me.Shape = new fk_Block(1.0, 1.0, 1.0);
                            Me.Material = fk_Material.Red;
                            Me.BMode = fk_BoundaryMode.OBB;
                            Me.AdjustOBB();
                            Me.GlTranslate(new fk_Vector(j - MapWidth / 2, 1, i - MapHeight / 2));
                            argWindow.Entry(Me);
                            argWindow.CameraModel.Parent = Me;
                            argWindow.CameraPos = new fk_Vector(0, 20, 0);
                            argWindow.CameraFocus = new fk_Vector(0, 0, 0);


                            mecom++;
                        }
                    }
                    count1++;
                }
            }
        }
        static void Enenycreate(fk_AppWindow argWindow)
        {
            enemy = new fk_Model[5];
            int count = 0;
            int enemycom = 0;
            for (int i = 0; i < MapHeight; i++)
            {
                for (int j = 0; j < MapWidth; j++)
                {

                    if (Map[i, j] == road)
                    {

                        if (count >= fk_Math.Rand(10, 1000) && enemycom < 5)
                        {
                            enemy[enemycom] = new fk_Model();
                            enemy[enemycom].Shape = new fk_Block(1.0, 1.0, 1.0);
                            enemy[enemycom].Material = fk_Material.MatBlack;
                            enemy[enemycom].GlTranslate(new fk_Vector(j - MapWidth / 2, 1, i - MapHeight / 2));
                            argWindow.Entry(enemy[enemycom]);
                            count = 0;
                            enemycom++;
                        }
                        count++;
                    }

                }
            }
        }

        static void GoalCreate(fk_AppWindow argWindow)
        {
            int count = 0;
            int enemycom = 0;
            for (int i = 0; i < MapHeight; i++)
            {
                for (int j = 0; j < MapWidth; j++)
                {

                    if (Map[i, j] == road)
                    {

                        if (count >=  fk_Math.Rand(10, 1000) && enemycom < 1)
                        {
                            goal= new fk_Model();
                            goal.Shape = new fk_Block(1.0, 1.0, 1.0);
                            goal.Material = fk_Material.Green ;
                            goal.BMode = fk_BoundaryMode.OBB;
                            goal.AdjustOBB();
                            goal.GlTranslate(new fk_Vector(j - MapWidth / 2, 1, i - MapHeight / 2));
                            argWindow.Entry(goal);
                            count = 0;
                            enemycom++;
                        }
                        count++;
                    }

                }
            }
        }
        static void Main(string[] args)
        {
            var window = new fk_AppWindow();
            window.Size = new fk_Dimension(1200, 700);
            window.BGColor = new fk_Color(0.8, 0.9, 1.0);
            window.TrackBallMode = true;
            window.ShowGuide(fk_Guide.GRID_XZ);
            window.Open();

            bool Push = true;

            if (Flag == true)
            {
                
                ResetMapData(window);

                CreateSpaceData();

                CreateDangeon(window);

                Mecreate(window);
                Enenycreate(window);
                GoalCreate(window);
                
                Flag = false;
            }
            double t = 0;
            
            while (window.Update())
            {
                if (Me.IsInter(goal) == true)
                {
                    if (window.GetKeyStatus('z', fk_Switch.DOWN) == true && Push == true)
                    {
                        ResetMapData(window);

                        CreateSpaceData();

                        CreateDangeon(window);

                        Mecreate(window);
                        Enenycreate(window);
                        GoalCreate(window);
                        Push = false;
                        Flag = false;
                    }
                    //window.ClearModel(); // 全 て の 登 録 を 解 除
                    
                    // modelA と modelB は 干 渉 し て い る 。
                }


                if (window.GetKeyStatus(fk_Key.RIGHT, fk_Switch.DOWN) == true && Push == true)
                {
                    for (int Num = 0; Num < model.Length; Num++)
                    {
                        model[Num].SnapShot();
                    }
                    Me.SnapShot();
                    Me.LoTranslate(1.0, 0.0, 0.0);
                    /*for (int Num = 0; Num < model.Length; Num++)
                    {
                        if (Me.IsCollision(model[Num], time: ref t) == true)
                        {
                            Me.Restore(t);
                        }
                        // modelA と modelB は 干 渉 し て い る 。
                    }*/
                    Push = false;
                }
                if (window.GetKeyStatus(fk_Key.LEFT, fk_Switch.DOWN) == true && Push == true)
                {
                    for (int Num = 0; Num < model.Length; Num++)
                    {
                        model[Num].SnapShot();
                    }
                    Me.SnapShot();
                    Me.LoTranslate(-1.0, 0.0, 0.0);
                    
                    Push = false;
                }
                if (window.GetKeyStatus(fk_Key.UP, fk_Switch.DOWN) == true && Push == true)
                {
                    for (int Num = 0; Num < model.Length; Num++)
                    {
                        model[Num].SnapShot();
                    }
                    Me.SnapShot();
                    Me.LoTranslate(0.0, 0.0, -1.0);
                    
                    Push = false;
                }
                if (window.GetKeyStatus(fk_Key.DOWN, fk_Switch.DOWN) == true && Push == true)
                {
                    for (int Num = 0; Num < model.Length; Num++)
                    {
                        model[Num].SnapShot();
                    }
                    Me.SnapShot();
                    Me.LoTranslate(0.0, 0.0, 1.0);
                    /* for (int Num = 0; Num < model.Length; Num++)
                     {
                         if (Me.IsCollision(model[Num], time: ref t) == true)
                         {
                             Me.Restore(t);
                         }
                         // modelA と modelB は 干 渉 し て い る 。
                     }*/
                    Push = false;
                }
                if (window.GetKeyStatus('z', fk_Switch.DOWN) == true && Push == true)
                {
                    Attack(window);

                    Push = false;
                }
                if (window.GetKeyStatus('x', fk_Switch.DOWN) == true && Push == true)
                {
                    buildMagic();
                    Push = false;
                }
                if (window.GetKeyStatus(fk_Key.RIGHT, fk_Switch.UP) == true && Push == false)
                {
                    Push = true;

                }
                if (window.GetKeyStatus(fk_Key.LEFT, fk_Switch.UP) == true && Push == false)
                {
                    Push = true;

                }
                if (window.GetKeyStatus(fk_Key.UP, fk_Switch.UP) == true && Push == false)
                {
                    Push = true;

                }
                if (window.GetKeyStatus(fk_Key.DOWN, fk_Switch.UP) == true && Push == false)
                {
                    Push = true;

                }
                if (window.GetKeyStatus('z', fk_Switch.UP) == true && Push == false)
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        Console.WriteLine(i);
                        if (i > 500)
                        {

                            window.Remove(AttackObj);
                        }

                    }
                    Push = true;
                }
                if (window.GetKeyStatus('x', fk_Switch.UP) == true && Push == false)
                {
                    Push = true;
                }
                if (window.GetKeyStatus(fk_Key.ENTER, fk_Switch.DOWN) == true && Push == true)
                {
                    ResetMapData(window);

                    CreateSpaceData();

                    CreateDangeon(window);

                    Mecreate(window);
                    Enenycreate(window);
                    GoalCreate(window);
                    Push = false;
                }//チートコード
                if (window.GetKeyStatus(fk_Key.ENTER, fk_Switch.UP) == true && Push == false)
                {
                    Push = true;

                }
            }
        }
        static void Attack(fk_AppWindow argWindow)
        {
            Console.WriteLine("攻撃");
            AttackObj = new fk_Model();
            AttackObj.Shape=new fk_Block(3.0,1.0,1.0);
            AttackObj.Material = fk_Material.Red;
            AttackObj.GlTranslate(new fk_Vector(Me.Position.x, Me.Position.y, Me.Position.z));
            argWindow.Entry(AttackObj);
            


        }
        static void buildMagic()
        {
            Console.WriteLine("魔法構築");
        }
    }

    /*
     FKを使ってプログラミング
     ダンジョンのランダム生成から自機、敵の自動配置まで可能
     攻撃や敵との接触判定を取りたかったが上手くできなかった。
     */
}
