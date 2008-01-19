﻿/*
    Copyright 2007, Joe Davidson <joedavidson@gmail.com>

    This file is part of FFTPatcher.

    FFTPatcher is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    FFTPatcher is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with FFTPatcher.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Windows.Forms;
using FFTPatcher.Datatypes;

namespace FFTPatcher.Editors
{
    public partial class CommonAbilitiesEditor : UserControl
    {
        private static readonly string[] PropertiesNames = new string[] { 
            "LearnWithJP", "Action", "LearnOnHit", "Blank1", 
            "Unknown1", "Unknown2", "Unknown3", "Blank2", 
            "Blank3", "Blank4", "Blank5", "Unknown4" };
        private static readonly string[] AIPropertyNames = new string[] {
            "AIHP", "AIMP", "AICancelStatus", "AIAddStatus", "AIStats", "AIUnequip", "AITargetEnemies", "AITargetAllies",
            "AIIgnoreRange", "AIReflectable", "AIUndeadReverse", "AIUnknown1", "AIRandomHits", "AIUnknown2", "AIUnknown3", "AISilence",
            "AIBlank", "AIDirectAttack", "AILineAttack", "AIVerticalIncrease", "AITripleAttack", "AITripleBracelet", "AIMagicDefenseUp", "AIDefenseUp" };

        private Ability ability;
        public Ability Ability
        {
            get { return ability; }
            set
            {
                if( ability != value )
                {
                    ability = value;
                    UpdateView();
                }
            }
        }

        bool ignoreChanges = false;

        public CommonAbilitiesEditor()
        {
            InitializeComponent();

            jpCostSpinner.ValueChanged +=
                delegate( object sender, EventArgs e )
                {
                    if( !ignoreChanges )
                        ability.JPCost = (UInt16)jpCostSpinner.Value;
                };
            chanceSpinner.ValueChanged +=
                delegate( object sender, EventArgs e )
                {
                    if( !ignoreChanges )
                        ability.LearnRate = (byte)chanceSpinner.Value;
                };
            abilityTypeComboBox.SelectedIndexChanged +=
                delegate( object sender, EventArgs e )
                {
                    if( !ignoreChanges )
                        ability.AbilityType = (AbilityType)abilityTypeComboBox.SelectedIndex;
                };
            propertiesCheckedListBox.ItemCheck += CheckedListBox_ItemCheck;
            aiCheckedListBox.ItemCheck += CheckedListBox_ItemCheck;
        }

        private void CheckedListBox_ItemCheck( object sender, ItemCheckEventArgs e )
        {
            if( !ignoreChanges )
            {
                CheckedListBox clb = sender as CheckedListBox;
                if( clb == propertiesCheckedListBox )
                {
                    SetAbilityFlag( PropertiesNames[e.Index], e.NewValue == CheckState.Checked );
                }
                else if( clb == aiCheckedListBox )
                {
                    SetAbilityFlag( AIPropertyNames[e.Index], e.NewValue == CheckState.Checked );
                }
            }
        }

        private bool GetFlagFromAbility( string name )
        {
            return ReflectionHelpers.GetFlag( ability, name );
        }

        private void SetAbilityFlag( string name, bool newValue )
        {
            ReflectionHelpers.SetFlag( ability, name, newValue );
        }

        private Context ourContext = Context.Default;

        private void UpdateView()
        {
            this.SuspendLayout();
            ignoreChanges = true;

            if( ourContext != FFTPatch.Context )
            {
                ourContext = FFTPatch.Context;
                aiCheckedListBox.Items.Clear();
                aiCheckedListBox.Items.AddRange( ourContext == Context.US_PSP ? Resources.AbilityAI : PSXResources.AbilityAI );
                abilityTypeComboBox.DataSource = ourContext == Context.US_PSP ? Resources.AbilityTypes : PSXResources.AbilityTypes;
            }

            jpCostSpinner.SetValueAndDefault( ability.JPCost, ability.Default.JPCost );
            chanceSpinner.SetValueAndDefault( ability.LearnRate, ability.Default.LearnRate );

            abilityTypeComboBox.SetValueAndDefault(
                abilityTypeComboBox.Items[(byte)ability.AbilityType],
                abilityTypeComboBox.Items[(byte)ability.Default.AbilityType] );

            if( ability.Default != null )
            {
                propertiesCheckedListBox.SetValuesAndDefaults( ReflectionHelpers.GetFieldsOrProperties<bool>( ability, PropertiesNames ), ability.Default.PropertiesToBoolArray() );
                bool[] bools = ability.Default.AIFlags.ToBoolArray();

                aiCheckedListBox.SetValuesAndDefaults( ReflectionHelpers.GetFieldsOrProperties<bool>( ability, AIPropertyNames ), ability.Default.AIFlags.ToBoolArray() );
            }

            ignoreChanges = false;
            this.ResumeLayout();
        }
    }
}