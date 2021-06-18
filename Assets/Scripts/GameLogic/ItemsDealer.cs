using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace LabyrinthGame
{
    namespace GameLogic
    {
        public class ItemsDealer
        {
            private static int CardsInHand = 6;
            private static int PlayersNumber = 4;

            public ItemsDealer(int itemsSeed)
            {
                m_randomizer = new System.Random(itemsSeed);
            }
            public void DealItems(IList<Player> players)
            {
                if (players == null)
                {
                    throw new ArgumentNullException("Players list can not be null");
                }
                if (players.Count != PlayersNumber)
                {
                    throw new ArgumentException("Four players expected");
                }
                if (players.Contains(null))
                {
                    throw new ArgumentNullException("Player can not be null");
                }

                var itemsList = new List<Labyrinth.Item>((Labyrinth.Item[])Enum.GetValues(typeof(Labyrinth.Item)));
                itemsList.Remove(Labyrinth.Item.None);
                itemsList.Remove(Labyrinth.Item.Home);

                Utils.Randomization.Shuffle(itemsList, m_randomizer);

                for (var i = 0; i < PlayersNumber; ++i)
                {
                    players[i].ItemsToFind = itemsList.GetRange(i * CardsInHand, CardsInHand);
                    players[i].ItemsToFind.Add(Labyrinth.Item.Home);
                }                
            }

            private System.Random m_randomizer;
        }

    } //namespace GameLogic

} //namespace LabyrinthGame
