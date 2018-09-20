using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Mahjong
{
    internal class Program
    {
        private struct Card
        {
            /// <summary>
            /// 条、筒、万、风：1，2，3，4
            /// </summary>
            public int Type;
            /// <summary>
            /// 牌面具体数值
            /// 当类型为风（4）时 1-7 分别代表：东南西北中发白
            /// </summary>
            public int Num;
        }

        // Use this for initialization
        private void Start()
        {
            List<Card> initCards = GenerateList(new[] {33, 33, 33, 24, 24, 24, 34, 34, 34, 44, 44, 53, 53, 53,53});
            Console.WriteLine(MaJangIsOver(initCards) ? "Win" : "Can not Win");
        }


        /// <summary>
        /// 通过数组生成牌序列
        /// </summary>
        /// <param name="cardGenerator"></param>
        /// <returns></returns>
        static List<Card> GenerateList(int[] cardGenerator)
        {
            var cards = new List<Card>();
            foreach (var t in cardGenerator)
            {
                int type = t.ToString()[1];
                int num = t.ToString()[0];
                
                var c = new Card
                {
                    Type = type, 
                    Num = num
                };
                
                cards.Add(c);
            }
            
            return cards;
        }

        /// <summary>
        /// 判胡
        /// </summary>
        /// <param name="originalCards"></param>
        /// <returns></returns>
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
                        newCard.Num = originalCards[j].Num;
                        newCard.Type = originalCards[j].Type;
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
                int listCount = RemoveAllThree(allCards);
                if (listCount == 0)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// 去除所有顺子、对子、杠
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        int RemoveAllThree(List<Card> cards)
        {
            List<Card> project = new List<Card>();
            int minCount = 100;

            for (int i = 0; i < 6; i++)
            {
                project.Clear();
                
                foreach (var v in cards)
                {
                    project.Add(v);
                }
                
                switch (i)
                {
                    case 0:
                        project = RemoveAllExposed(project);
                        project = RemoveAllPair(project);
                        project = RemoveAllStraight(project);
                        break;
                    case 1:
                        project = RemoveAllExposed(project);
                        project = RemoveAllStraight(project);
                        project = RemoveAllPair(project);
                        break;
                    case 2:
                        project = RemoveAllPair(project);
                        project = RemoveAllExposed(project);
                        project = RemoveAllStraight(project);
                        break;
                    case 3:
                        project = RemoveAllPair(project);
                        project = RemoveAllStraight(project);
                        project = RemoveAllExposed(project);
                        break;
                    case 4:
                        project = RemoveAllStraight(project);
                        project = RemoveAllExposed(project);
                        project = RemoveAllPair(project);
                        break;
                    case 5:
                        project = RemoveAllStraight(project);
                        project = RemoveAllPair(project);
                        project = RemoveAllExposed(project);
                        break;
                }

                minCount = Math.Min(project.Count, minCount);
                if (minCount == 0)
                {
                    return minCount;
                }
            }


            return minCount;
        }


        /// <summary>
        /// 移除顺子
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        static List<Card> RemoveAllStraight(List<Card> cards)
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

        /// <summary>
        /// 移除所有杠
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        private List<Card> RemoveAllExposed(List<Card> cards)
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
                        for (int l = 0; l < cards.Count; l++)
                        {
                            if (l == k || l == i || l == j)
                                continue;
                            if (IsSameFour(cards[i], cards[j], cards[k], cards[l]))
                            {
                                cards.RemoveAt(i);
                                cards.RemoveAt(j - 1);
                                cards.RemoveAt(k - 2);
                                cards.RemoveAt(l - 3);
                                RemoveAllExposed(cards);
                            }
                        }
                    }
                }
            }

            return cards;
        }

        /// <summary>
        /// 移除所有对子
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        private List<Card> RemoveAllPair(List<Card> cards)
        {
            if (cards == null) throw new ArgumentNullException(nameof(cards));
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
        static bool IsNeighbor(Card c1, Card c2, Card c3)
        {
            //风不能有顺子
            if (c1.Type == 4 || c2.Type == 4 || c3.Type == 4)
            {
                return false;
            }

            //不同类型不能有顺子
            if (!(c1.Type == c2.Type && c1.Type == c3.Type))
            {
                return false;
            }

            //三个数字不能相同
            if (!(c1.Num != c2.Num && c1.Num != c3.Num))
            {
                return false;
            }

            return Math.Min(c1.Num, Math.Min(c2.Num, c3.Num)) + 2 == Math.Max(c1.Num, Math.Max(c2.Num, c3.Num));
        }

        /// <summary>
        /// 判断是否是杠
        /// </summary>
        /// <returns></returns>
        static bool IsSameFour(Card c1, Card c2, Card c3, Card c4)
        {
            if (IsSameCard(c1, c2) && IsSameCard(c2, c3) && IsSameCard(c3, c4))
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// 判断三张牌牌面是否相同
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="c3"></param>
        /// <returns></returns>
        static bool IsSameThree(Card c1, Card c2, Card c3)
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
        static bool IsSameCard(Card c1, Card c2)
        {
            return c1.Type == c2.Type && c1.Num == c2.Num;
        }


        /// <summary>
        /// 指定 List 中是否含有某张牌
        /// </summary>
        /// <param name="c"></param>
        /// <param name="cards"></param>
        /// <returns></returns>
        bool HaveThisCardInList(Card c, List<Card> cards)
        {
            foreach (var t in cards)
            {
                if (IsSameCard(c, t))
                {
                    return true;
                }
            }

            return false;
        }

        public static void Main(string[] args)
        {
            var sp=new Stopwatch();
            sp.Start();
            new Program().Start();
            sp.Stop();
            Console.WriteLine($"total running time : {sp.Elapsed.TotalSeconds}");
        }
    }
}