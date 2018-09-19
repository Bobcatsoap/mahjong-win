using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Mahjong
{
    internal class Program
    {
        //条、筒、万 1，2，3
        //风和箭 4
        //num 数字 1-9 代表对应牌
        //type 为 4 时 num 的值 1-7 分别代表：东南西北中发白

        struct Card
        {
            public int type;
            public int num;

            public Card(int type, int num)
            {
                this.type = type;
                this.num = num;
            }


            public void PrintCardValue()
            {
                string type = "";
                bool type4 = false;
                switch (this.type)
                {
                    case 1:
                        type = "条";
                        break;
                    case 2:
                        type = "筒";
                        break;
                    case 3:
                        type = "万";
                        break;
                    case 4:
                        type = "";
                        type4 = true;
                        break;
                }

                string num = "";

                if (type4)
                {
                    switch (this.num)
                    {
                        case 1:
                            num = "东";
                            break;
                        case 2:
                            num = "西";
                            break;
                        case 3:
                            num = "南";
                            break;
                        case 4:
                            num = "北";
                            break;
                        case 5:
                            num = "中";
                            break;
                        case 6:
                            num = "发";
                            break;
                    }
                }
                else
                {
                    num = this.num.ToString();
                }
//
//                Console.WriteLine(num);
//                Console.WriteLine(type);
            }
        }

        // Use this for initialization
        void Start()
        {
            List<Card> initCards = GenerateList(new int[] {34,33,42,42,42,43,43,43,44,44,44,35,35,35 });
            if (MaJangIsOver(initCards))
            {
                Console.WriteLine("Win");
            }
            else
            {
                Console.WriteLine("Can not Win");
            }
        }

        List<Card> GenerateList(int[] cardGenerator)
        {
            List<Card> cards = new List<Card>();
            for (int i = 0; i < cardGenerator.Length; i++)
            {
                int type = cardGenerator[i].ToString()[0];
                int num = cardGenerator[i].ToString()[1];
                Card c = new Card();
                c.type = type;
                c.num = num;
                cards.Add(c);
            }

            foreach (Card ca in cards)
            {
                ca.PrintCardValue();
            }

            return cards;
        }

        bool MaJangIsOver(List<Card> originalCards)
        {
            if (originalCards.Count < 14)
            {
                return false;
            }

            //已检查过的两张
            List<Card> whoHaveBothCard = new List<Card>();


            for (int i = 0; i < originalCards.Count; i++)
            {
                Card c = originalCards[i];
                //如果检查过的列表中存在该牌就跳过
                if (HaveThisCardInList(c, whoHaveBothCard))
                {
                    continue;
                }

                //寻找相同牌
                for (int j = 0; j < originalCards.Count; j++)
                {
                    if (j == i)
                        continue;

                    //
                    if (IsSameCard(originalCards[i], originalCards[j]))
                    {
                        Card newCard = new Card();
                        newCard.num = originalCards[j].num;
                        newCard.type = originalCards[j].type;
                        whoHaveBothCard.Add(newCard);
                        break;
                    }
                }
            }

            //去重时防止多次去处同一对儿
            List<Card> whoIsMarkHaveToRemove = new List<Card>();

            //有几对两张相同牌就有几种可能性
            for (int i = 0; i < whoHaveBothCard.Count; i++)
            {
                List<Card> allCards = new List<Card>();

                //
                foreach (var c in originalCards)
                {
                    allCards.Add(c);
                }


                int rmCardIndex = -1;
                int rmCardIndex2 = -1;
                //本次移除操作是否完成
                bool oneRemoveOver = false;
                for (int j = 0; j < allCards.Count; j++)
                {
                    if (HaveThisCardInList(allCards[j], whoHaveBothCard))
                    {
                        //如果已标记列表中没有该牌
                        if (!HaveThisCardInList(allCards[j], whoIsMarkHaveToRemove))
                        {
                            whoIsMarkHaveToRemove.Add(allCards[j]);
                            for (int k = 0; k < allCards.Count; k++)
                            {
                                if (IsSameCard(allCards[j], allCards[k]))
                                {
                                    if (rmCardIndex == -1)
                                    {
                                        rmCardIndex = k;
                                    }

                                    if (rmCardIndex2 == -1)
                                    {
                                        rmCardIndex2 = k;
                                    }
                                }
                            }

                            allCards.RemoveAt(rmCardIndex);
                            allCards.RemoveAt(rmCardIndex2);
                            break;
                        }
                    }
                }

                //判别
                List<Card> lastCards = RemoveAllThree(allCards);
                if (lastCards.Count <= 0)
                {
                    return true;
                }
            }

            return false;
        }


        //递归去掉所有顺子和对子
        List<Card> RemoveAllThree(List<Card> cards)
        {
            cards = RemoveAllStraight(cards);
            cards = RemoveAllPair(cards);
            return cards;
        }


        //移除顺子
        List<Card> RemoveAllStraight(List<Card> cards)
        {
            //移除所有顺子
            for (int i = 0; i < cards.Count; i++)
            {
                for (int j = 0; j < cards.Count; j++)
                {
                    if (i == j)
                        continue;
                    for (int k = 0; k < cards.Count; k++)
                    {
                        if (k == i || k == j)
                            continue;
                        if (IsNeighbor(cards[i], cards[j], cards[k]))
                        {
                            cards.RemoveAt(i);
                            cards.RemoveAt(j - 1);
                            cards.RemoveAt(k - 2);
                            RemoveAllStraight(cards);
                        }
                    }
                }
            }

            return cards;
        }

        List<Card> RemoveAllPair(List<Card> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                for (int j = 0; j < cards.Count; j++)
                {
                    if (i == j)
                        continue;
                    for (int k = 0; k < cards.Count; k++)
                    {
                        if (k == i || k == j)
                            continue;
                        if (IsSameThree(cards[i], cards[j], cards[k]))
                        {
                            cards.RemoveAt(i);
                            cards.RemoveAt(j - 1);
                            cards.RemoveAt(k - 2);
                            RemoveAllPair(cards);
                        }
                    }
                }
            }

            return cards;
        }


        /// <summary>
        /// 判断三张牌是否能组成顺子,风牌箭牌不能组成顺子
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="c3"></param>
        /// <returns></returns>
        bool IsNeighbor(Card c1, Card c2, Card c3)
        {
            //风不能有顺子
            if (c1.type == 4 || c2.type == 4 || c3.type == 4)
            {
                return false;
            }

            //不同类型不能有顺子
            if (!(c1.type == c2.type && c1.type == c3.type))
            {
                return false;
            }

            //三个数字不能相同
            if (!(c1.num != c2.num && c1.num != c3.num))
            {
                return false;
            }

            return Math.Min(c1.num, Math.Min(c2.num, c3.num)) + 2 == Math.Max(c1.num, Math.Max(c2.num, c3.num));
        }


        /// <summary>
        /// 判断三张牌牌面是否相同
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="c3"></param>
        /// <returns></returns>
        bool IsSameThree(Card c1, Card c2, Card c3)
        {
            if (IsSameCard(c1, c2) && IsSameCard(c2, c3))
                return true;
            return false;
        }

        /// <summary>
        /// 判断两张牌牌面是否一样
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        bool IsSameCard(Card c1, Card c2)
        {
            return c1.type == c2.type && c1.num == c2.num;
        }

        bool HaveThisCardInList(Card c, List<Card> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (IsSameCard(c, cards[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public static void Main(string[] args)
        {
            new Program().Start();
        }
    }
}