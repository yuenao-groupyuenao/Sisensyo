using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class LogicSisensyo : MonoBehaviour
{

    public GameObject originObject;
    public static GameObject staticOriginObject;

    public GameObject gameBoard;

    private static int tileWidth;
    private static int tileHeight;

    private static int margin = 4;

    private static int tileNum { get; set; }
    private static int fieldWidth { get; set; }
    private static int fieldHeight { get; set; }

    public State actualState;

    System.Random random = new System.Random();

    private static readonly int EASY_TILE_WIDTH = 7;
    private static readonly int EASY_TILE_HEIGHT = 4;
    private static readonly int NORMAL_TILE_WIDTH = 17;
    private static readonly int NORMAL_TILE_HEIGHT = 8;

    private SESpeaker sESpeaker;


    public class State
    {
        public List<int> rawBoardData { get; set; }
        public List<GameObject> boardView { get; set; }
        public int targetIndex { get; set; }
        public int rest { get; set; }
        public Tuple<int, int> hintPairIndex { get; set; }

        public State(List<int> rawBoardData)
        {
            this.rawBoardData = rawBoardData;
            this.boardView = new List<GameObject>();
            this.targetIndex = -1;
            this.rest = tileNum;
            this.hintPairIndex = new Tuple<int, int>(-1,-1);
        }

        public State DeepCopy()
        {
            State copyState = new State(new List<int>(rawBoardData));
            return copyState;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        staticOriginObject = originObject;
        GameModeToPaiCount();
        InitFieldVar();
        actualState = MakeField();

        // 生成された配置が詰んでないかの判定は一旦おいておく
        //for (int i = 0; i < 50000; i++)
        //{
        //    if (Solve(actualState.DeepCopy()))
        //    {
        //        break;
        //    }
        //}

        InitView(actualState);

        sESpeaker = GameObject.Find("SESpeakerObject").GetComponent<SESpeaker>();
    }

    /// <summary>
    /// Viewの初期化。後で分離する。
    /// </summary>
    /// <param name="state"></param>
    void InitView(State state)
    {
        for (int i = 0; i < state.rawBoardData.Count; i++)
        {
            state.boardView.Add(MakePaiView(i, state.rawBoardData[i]));
        }
        ScaleGameBoardView();
    }

    void GameModeToPaiCount()
    {
        switch (ScoreManager.gameMode)
        {
            case Gamemode.Easy:
                tileWidth = EASY_TILE_WIDTH;
                tileHeight = EASY_TILE_HEIGHT;
                return;

            case Gamemode.Normal:
                tileWidth = NORMAL_TILE_WIDTH;
                tileHeight = NORMAL_TILE_HEIGHT;
                return;
        }
    }

    void InitFieldVar()
    {
        tileNum = tileWidth * tileHeight;
        fieldWidth = tileWidth + margin;
        fieldHeight = tileHeight + margin;
    }

    State MakeField()
    {
        // 麻雀牌を作成する
        List<int> tilesMaterial = Range(0, tileNum, 1);
        List<int> tiles = new List<int>();
        for (int i = 0; i < tilesMaterial.Count; i++)
        {
            tiles.Add(1 + (tilesMaterial[i] / 4));
        }

        // 盤を作成する
        List<int> rawBoardData = new List<int>();
        int boardAllSize = Range(0, fieldWidth * fieldHeight, 1).Count;
        for (int i = 0; i < boardAllSize; i++)
        {
            int posX = IndexToX(i);
            int posY = IndexToY(i);
            int revPosX = fieldWidth - 1 - posX;
            int revPosY = fieldHeight - 1 - posY;

            int mostPosMin = System.Math.Min(System.Math.Min(posX, posY), System.Math.Min(revPosX, revPosY));

            int result = 0;

            if (mostPosMin == 0)
            {
                //-1は壁
                result = -1;
            }
            else
            {
                if (mostPosMin == 1)
                {
                    //0は空
                    result = 0;
                }
                else
                {
                    result = PickupFromList(tiles);
                }
            }
            rawBoardData.Add(result);
        }
        return new State(rawBoardData);
    }

    /// <summary>
    /// 画面上の麻雀牌を作成する
    /// </summary>
    /// <param name="index"></param>
    /// <param name="paiTypeId"></param>
    /// <returns></returns>
    public GameObject MakePaiView(int index, int paiTypeId)
    {
        int posX = IndexToX(index);
        int posY = IndexToY(index);
        float viewPosX = (float)(posX * ViewEnvironment.paiMarginWidth + ViewEnvironment.paiFirstPOSX);
        float viewPosY = (float)(posY * -ViewEnvironment.paiMarginHeight + ViewEnvironment.paiFirstPOSY);
        float viewPosZ = (float)(ViewEnvironment.paiPOSZ);

        GameObject paiObject = Instantiate(staticOriginObject, new Vector3(viewPosX, viewPosY, viewPosZ), Quaternion.identity);
        paiObject.transform.parent = gameBoard.transform;

        Pai targetpai = paiObject.GetComponent<Pai>();
        targetpai.paiTypeId = paiTypeId;
        targetpai.index = index;

        paiObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = MakeDisplayChar(paiTypeId);

        //壁か空白なら透明にする。
        if (paiTypeId < 1)
        {
            paiObject.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 1.0f);
        }
        return paiObject;
    }

    string MakeDisplayChar(int paiTypeId)
    {
        if (paiTypeId < 1)
        {
            //-1か0は牌じゃない
            return "";
        }
        else if (paiTypeId < 8)
        {
            //字牌
            return "東南西北中發 "[paiTypeId - 1].ToString();
        }
        else if (paiTypeId < 17)
        {
            //萬子
            return "一二三四五六七八九"[paiTypeId - 8].ToString() + "\r\n萬";
        }
        else if (paiTypeId < 26)
        {
            //筒子
            return "一二三四五六七八九"[paiTypeId - 17].ToString() + "\r\n筒";
        }
        else
        {
            //索子
            return "一二三四五六七八九"[paiTypeId - 26].ToString() + "\r\n索";
        }
    }

    void ScaleGameBoardView()
    {
        int scaleRatio = NORMAL_TILE_WIDTH / tileWidth;
        Debug.Log(gameBoard.transform.parent);
        gameBoard.transform.parent.transform.localScale = new Vector3(scaleRatio, scaleRatio, 1);
    }

}

public partial class LogicSisensyo
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            GameObject clickedGameObject = null;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                clickedGameObject = hit.collider.gameObject;
            }

            if (clickedGameObject == null)
            {
                Debug.Log("無をクリックしています");
            }
            else
            {
                Pai clickedPai = clickedGameObject.GetComponent<Pai>();

                if(0 < clickedPai.paiTypeId )
                {
                    Debug.Log("クリックされました。" +
                        " index:" + clickedPai.index
                        + " paiTypeId: " + clickedPai.paiTypeId
                        );
                    UpdateState(actualState, clickedPai.index);
                }
            }

            UpdateView(actualState);
        }
        if (CheckClear())
        {
            ScoreManager.isClear = true;
            TransResult();
        }

    }

    void UpdateView(State state)
    {
        for(int i = 0;i < state.boardView.Count; i++)
        {
            UpdatePaiView(i, state.boardView[i]);
        }
    }

    void UpdatePaiView(int index, GameObject paiObject)
    {
        //存在しなければ透明
        if (actualState.rawBoardData[index] < 1)
        {
            UpdatePaiData(paiObject, actualState.rawBoardData[index], new Color(0, 0, 0, 1.0f), "");
        }
        else
        {
            //選択されていたら選択色
            if (index == actualState.targetIndex)
            {
                UpdatePaiColor(paiObject, Color.cyan);
            }
            //ヒントのindexならヒント色
            else if (index == actualState.hintPairIndex.Item1 ||
                     index == actualState.hintPairIndex.Item2)
            {
                UpdatePaiColor(paiObject, Color.red);
            }
            //どれでもなければ通常色
            else
            {
                //TODO:Viewを分離するときは定数化する
                UpdatePaiColor(paiObject, staticOriginObject.GetComponent<MeshRenderer>().material.color);
            }
        }
    }

    void UpdatePaiData(GameObject paiObject, int paiTypeId, Color color, String text)
    {
        paiObject.GetComponent<Pai>().paiTypeId = paiTypeId;
        UpdatePaiColor(paiObject, color);
        paiObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = text;
    }

    void UpdatePaiColor(GameObject paiObject, Color color)
    {
        paiObject.GetComponent<MeshRenderer>().material.color = color;
    }


    State UpdateState(State state, int clickPointIndex)
    {
        //壁か空白ならなにもしない
        if (clickPointIndex < 0)
        {
            return state;
        }

        // targetが設定されていなければ設定して返す
        if (state.targetIndex < 0)
        {
            state.targetIndex = clickPointIndex;
            return state;
        }

        // 二角で取れるか探索してペアでなければtargetの座標を消去
        if (!TestPair(state.rawBoardData, state.targetIndex, clickPointIndex))
        {
            state.targetIndex = -1;
            return state;
        }

        sESpeaker.playSE();

        //二角で取れれば、取れた箇所を消去する処理を行う
        for (int i = 0; i < state.boardView.Count; i++)
        {
            //クリックした箇所かtargetならばその場所を空白に上書きする
            if (i == clickPointIndex || i == state.targetIndex)
            {
                Debug.Log("Delete:"  + i);
                state.rawBoardData[i] = 0;
            }
        }
        state.targetIndex = -1;
        state.rest -= 2;

        return state;

    }

    /// <summary>
    /// 空白以外に当たるまで再帰で探索。
    /// 空白にあたったらその一歩手前のindexを返す。
    /// <param name="board">盤面の状態の配列</param>
    /// <param name="targetIndex">選択中の座標</param>
    /// <param name="step">探索のステップ数</param>
    /// </summary>
    int GetSidePassableIndex(List<int> rawBoardData, int targetIndex, int step)
    {
        if (rawBoardData[targetIndex + step] != 0)
        {
            return targetIndex;
        }
        else
        {
            return GetSidePassableIndex(rawBoardData, targetIndex + step, step);
        }
    }

    /// <summary>
    /// 2つの牌の間に線をつなげるか判定する。
    /// XYを入れ替えて2回実行しないと、全て判定できないため注意
    /// </summary>
    /// <param name="board"></param>
    /// <param name="firstTargetIndex"></param>
    /// <param name="secondTargetIndex"></param>
    /// <param name="funcIndexToXorY1"></param>
    /// <param name="funcIndexToXorY2"></param>
    /// <param name="funcXYorYXToIndex"></param>
    /// <returns></returns>
    Boolean CheckPass(List<int> rawBoardData, int firstTargetIndex, int secondTargetIndex, Func<int, int> funcIndexToXorY1, Func<int, int> funcIndexToXorY2, Func<int, int, int> funcXYorYXToIndex)
    {
        int vector = funcXYorYXToIndex(1, 0);
        //経路探索のため、探索可能(牌の間に線を引くことが可能)な最大の大きさの四角形を作る。
        //それぞれの四隅のX値、Y値を算出する（XY軸は入れ替え可能なメソッドのためUV軸としている）
        int u0 = Math.Max(funcIndexToXorY1(GetSidePassableIndex(rawBoardData, firstTargetIndex, -vector)), funcIndexToXorY1(GetSidePassableIndex(rawBoardData, secondTargetIndex, -vector)));
        int u1 = Math.Min(funcIndexToXorY1(GetSidePassableIndex(rawBoardData, firstTargetIndex, vector)), funcIndexToXorY1(GetSidePassableIndex(rawBoardData, secondTargetIndex, vector)));
        int v0 = Math.Min(funcIndexToXorY2(firstTargetIndex), funcIndexToXorY2(secondTargetIndex)) + 1;
        int v1 = Math.Max(funcIndexToXorY2(firstTargetIndex), funcIndexToXorY2(secondTargetIndex)) - 1;

        //四角形の四隅からX軸、Y軸で探索可能な各範囲のリストを作る
        List<int> uAxisSerchableRange = Range(u0, u1 + 1, 1);
        List<int> vAxisSerchableRange = Range(v0, v1 + 1, 1);

        //四角形の空列を探索し判定を行う。
        for (int u = u0; u <= u1; u++)
        {
            Boolean isEmpty = true;
            for (int v = v0; v <= v1; v++)
            {
                //横軸(軸は可変)は四角形の四隅作成で探索済み（CheckSide）のため、縦軸のみ探索すればよい
                //縦軸にひとつでも空白以外があれば、isEmptyフラグをfalseにして次の縦軸の探索する
                if (rawBoardData[funcXYorYXToIndex(u, v)] != 0)
                {
                    isEmpty = false;
                    break;
                }
            }
            if (isEmpty)
            {
                //縦軸がすべて空白の箇所が存在
                //→二角以内で探索可能！ペアとして消去できる
                return true;
            }

        }
        //四角形をすべて回し、見つからなければfalseを返す
        return false;
    }

    Boolean TestPair(List<int> rawBoardData, int targetIndex, int clickPointIndex)
    {
        //同じところをクリックしたらfalse
        if (targetIndex == clickPointIndex)
        {
            return false;
        }

        //違う牌をクリックしたらfalse
        if (rawBoardData[targetIndex] != rawBoardData[clickPointIndex])
        {
            return false;
        }

        //XY軸を入れ替えて探索し、どちらかが通ればtrue
        if (CheckPass(rawBoardData, targetIndex, clickPointIndex, IndexToX, IndexToY, XYtoIndex) ||
            CheckPass(rawBoardData, targetIndex, clickPointIndex, IndexToY, IndexToX, YXtoIndex))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Boolean Solve(State state)
    {
        //解決済みペアを初期化
        solvedPairs = new Dictionary<int, List<int>> ();

        while (state.rest > 0)
        {
            Tuple<int, int> pairIndex = FindPair(state.rawBoardData);
            if(pairIndex != null)
            {
                state = UpdateState(state, pairIndex.Item1);
                state = UpdateState(state, pairIndex.Item2);
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    Dictionary<int,List<int>> solvedPairs = new Dictionary<int, List<int>>();

    Tuple<int,int> FindPair(List<int> rawBoardData)
    {
        int[] pair = { };
        for(int i = 0 ; i < rawBoardData.Count; i++)
        {
            int paiTypeId = rawBoardData[i];
            if(paiTypeId <= 0)
            {
                continue;
            }

            // 解決済みペアに対象の牌が登録されていなかったらindexをvalueで登録
            if (!solvedPairs.ContainsKey(paiTypeId))
            {
                solvedPairs.Add(paiTypeId, new List<int>());
                solvedPairs[paiTypeId].Add(i);
            }

            for (int j = 0; j < solvedPairs[paiTypeId].Count; j++)
            {
                int registerdIndex = solvedPairs[paiTypeId][j];
                if (TestPair(rawBoardData, i, registerdIndex))
                {
                    return Tuple.Create(i, registerdIndex);
                }
            }
            solvedPairs[paiTypeId].Add(i);
        }
        return null;
    }

    public void Hint()
    {
        //解決済みペアを初期化
        solvedPairs = new Dictionary<int, List<int>>();

        Tuple<int, int> pairIndex = FindPair(actualState.DeepCopy().rawBoardData);
        if(pairIndex != null)
        {
            actualState.hintPairIndex = pairIndex;
            UpdateView(actualState);
        }
    }

    public Boolean CheckClear()
    {
        for (int i = 0; i < actualState.rawBoardData.Count; i++)
        {
            if(0 < actualState.rawBoardData[i])
            {
                return false;
            }
        }
        return true;
    }

    public void TransResult()
    {
        SceneManager.LoadScene("Result");
    }
}

