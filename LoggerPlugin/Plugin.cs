using GoPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GoPlugin.Events;
using POGOProtos.Data;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Responses;

namespace LoggerPlugin
{
    public class Plugin : IPlugin
    {
        public override string PluginName { get; set; } = "ShadowProgr's Logger";

        //Subitems when hovering over plugin in menu
        public override IEnumerable<PluginDropDownItem> MenuItems { get; set; }

        private IEnumerable<IManager> _managers;

        public override async Task<bool> Load(IEnumerable<IManager> managers)
        {
            //Occurs when the plugin is loaded.

            _managers = managers;

            foreach (var manager in _managers)
            {
                manager.OnPokemonCaught += (sender, args) =>
                {
                    LogPokemon(manager, args);
                };
                manager.OnPokestopFarmed += (sender, args) =>
                {
                    LogPokestop(manager, args);
                };
            }

            await Task.Delay(0);

            return true;
        }

        public override async Task Run(IEnumerable<IManager> managers)
        {
            //Occurs when the plugin name is clicked in the menu.
            //managers parameter contains highlighted accounts
           


            await Task.Delay(0);

        }

        public override async Task<bool> Save()
        {
            //Occurs when closing. Occurs after settings are saved

            await Task.Delay(0);

            return true;
        }

        public void LogPokemon(IManager account, PokemonCaughtEventArgs args)
        {
            if (args.CatchResponse.Status != CatchPokemonResponse.Types.CatchStatus.CatchSuccess) return;
            using (var writer = new StreamWriter(@"SPLogs\Pokemon_" + account.AccountName + @".txt", true))
            {
                writer.WriteLine(DateTime.Now.ToString("dd-MM-yy HH:mm:ss") + " | " + "Caught a Pokémon: {0, -14}; IV: {1, -3}; CP: {2, -3}", args.Pokemon.PokemonId.ToString(), CalculateIv(args.Pokemon), args.Pokemon.Cp);
            }
        }

        public void LogPokestop(IManager account, EventArgs args)
        {
            var countSunStone = 0;
            var countKingsRock = 0;
            var countMetalCoat = 0;
            var countDragonScale = 0;
            var countUpGrade = 0;
            foreach (var item in account.AllItems)
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (item.ItemId)
                {
                    case ItemId.ItemSunStone:
                        countSunStone = item.Count;
                        break;
                    case ItemId.ItemKingsRock:
                        countKingsRock = item.Count;
                        break;
                    case ItemId.ItemMetalCoat:
                        countMetalCoat = item.Count;
                        break;
                    case ItemId.ItemDragonScale:
                        countDragonScale = item.Count;
                        break;
                    case ItemId.ItemUpGrade:
                        countUpGrade = item.Count;
                        break;
                }
            }
            using (var writer = new StreamWriter(@"SPLogs\Item_" + account.AccountName + @".txt", true))
            {
                writer.Write(DateTime.Now.ToString("dd-MM-yy HH:mm:ss") + " | " + "Sun Stones: {0, -2}; King's Rocks: {1, -2}; Metal Coats: {2, -2}; Dragon Scales: {3, -2}; Upgrades: {4, -2}", countSunStone, countKingsRock, countMetalCoat, countDragonScale, countUpGrade);
            }
        }

        public static int CalculateIv(PokemonData pokemon)
        {
            return (int)((pokemon.IndividualAttack + pokemon.IndividualDefense + pokemon.IndividualStamina) / 45.0 * 100);
        }
    }
}