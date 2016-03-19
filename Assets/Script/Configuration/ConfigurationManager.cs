using System;
using System.IO;
using System.Collections;
using SharpConfig;

using UnityEngine;

public class ConfigurationManager
{
    const string CONFIG_FILE_GAME = "Game.ini";
    const string CONFIG_FILE_PROJECTILE = "Projectile.ini";
    const string CONFIG_FILE_WEAPON = "Weapon.ini";
    const string CONFIG_FILE_SKILL = "Skill.ini";
    const string CONFIG_FILE_ENEMY = "Enemy.ini";
    const string CONFIG_FILE_QUEST = "Quest.ini";
    const string CONFIG_FILE_CHARACTER = "Character.ini";

    string PathDataConfig;
    string PathDataPersistentConfig;
    string PathGameConfigFile;
    string PathProjectileConfigFile;
    string PathWeaponConfigFile;
    string PathSkillConfigFile;
    string PathEnemyConfigFile;
    string PathQuestConfigFile;
    string PathCharacterConfigFile;

    public SharpConfig.Configuration GameConfiguration;
    public SharpConfig.Configuration ProjectileConfiguration;
    public SharpConfig.Configuration WeaponConfiguration;
    public SharpConfig.Configuration SkillConfiguration;
    public SharpConfig.Configuration EnemyConfiguration;
    public SharpConfig.Configuration QuestConfiguration;
    public SharpConfig.Configuration CharacterConfiguration;

    public ConfigurationManager()
    {
        PathDataConfig = Path.Combine(Application.dataPath, "Config");
        PathDataPersistentConfig = Path.Combine(Application.dataPath, "Config");

        PathGameConfigFile = Path.Combine(PathDataConfig, CONFIG_FILE_GAME);
        PathProjectileConfigFile = Path.Combine(PathDataConfig, CONFIG_FILE_PROJECTILE);
        PathWeaponConfigFile = Path.Combine(PathDataConfig, CONFIG_FILE_WEAPON);
        PathSkillConfigFile = Path.Combine(PathDataConfig, CONFIG_FILE_SKILL);
        PathEnemyConfigFile = Path.Combine(PathDataConfig, CONFIG_FILE_ENEMY);
        PathQuestConfigFile = Path.Combine(PathDataConfig, CONFIG_FILE_QUEST);
        PathCharacterConfigFile = Path.Combine(PathDataConfig, CONFIG_FILE_CHARACTER);
    }

    public void InitializeConfiguration()
    {
        if (!Directory.Exists(PathDataConfig))
            Directory.CreateDirectory(PathDataConfig);

        //InitializeGameConfiguration();
        InitializeProjectileConfiguration();
        InitializeWeaponConfiguration();
        //InitializeSkillConfiguration();
        //InitializeEnemyConfiguration();
        //InitializeQuestConfiguration();
        //InitializeCharacterConfiguration();
    }

    private void InitializeGameConfiguration()
    {
        if (File.Exists(PathGameConfigFile))
            GameConfiguration = SharpConfig.Configuration.LoadFromFile(PathGameConfigFile);
        else
        {
            GameConfiguration = new Configuration();

            GameConfigurationItem _item = new GameConfigurationItem();
            _item.CurrentVersion = new Version(0, 1);

            Section _currentSection = Section.FromObject("Game", _item);
            GameConfiguration.Add(_currentSection);
            GameConfiguration.SaveToFile(PathGameConfigFile);
        }
    }

    private void InitializeProjectileConfiguration()
    {
        if (File.Exists(PathProjectileConfigFile))
            ProjectileConfiguration = SharpConfig.Configuration.LoadFromFile(PathProjectileConfigFile);
        else
        {
            ProjectileConfiguration = new Configuration();

            for (int i = 1; i <= 4; i++)
            {
                ProjectileConfigItem _item = new ProjectileConfigItem();

                _item.ID = i;

                switch (i)
                {
                    case 1:
                        _item.PrefabPath = "Prefab/Projectile/Prototype_Pistol_Projectile";      
                        break;
                    case 2:
                        _item.PrefabPath = "Prefab/Projectile/Prototype_Rifle_Projectile";
                        break;
                    case 3:
                        _item.PrefabPath = "Prefab/Projectile/Prototype_Shotgun_Projectile";
                        break;
                    case 4:
                        _item.PrefabPath = "Prefab/Projectile/Prototype_SniperRifle_Projectile";
                        break;
                }

                Section _newSection = Section.FromObject(i.ToString(), _item);
                ProjectileConfiguration.Add(_newSection);
            }

            ProjectileConfiguration.SaveToFile(PathProjectileConfigFile);
        }
    }

    private void InitializeWeaponConfiguration()
    {
        if (File.Exists(PathWeaponConfigFile))
            WeaponConfiguration = SharpConfig.Configuration.LoadFromFile(PathWeaponConfigFile);
        else
        {
            WeaponConfiguration = new Configuration();

            for (int i = 1; i <= 4; i++)
            {
                string _sectionName = string.Empty;
                WeaponConfigurationItem _item = new WeaponConfigurationItem();

                _item.ID = i;

                switch (i)
                {
                    case 1:
                        _sectionName = "Prototype_Rifle";
                        _item.WeaponName = "Prototype Rifle";
                        _item.WeaponType = 0; // TODO: ALTERAR PARA ENUMERADOR!!!
                        _item.Automatic = true;
                        _item.ProjectileID = 2;
                        _item.AmmoMax = 30;
                        _item.Damage = 15;
                        _item.FirePerSecond = 12.5f;
                        _item.ReloadTimeInSeconds = 1.25f;
                        _item.PrefabPath = "Resources/Prefab/Weapon/Prototype_Rifle";
                        break;
                    case 2:
                        _sectionName = "Prototype_Shotgun";
                        _item.WeaponName = "Prototype Shotgun";
                        _item.WeaponType = 1; // TODO: ALTERAR PARA ENUMERADOR!!!
                        _item.Automatic = false;
                        _item.ProjectileID = 3;
                        _item.AmmoMax = 8;
                        _item.Damage = 50;
                        _item.FirePerSecond = 1.5f;
                        _item.ReloadTimeInSeconds = 3f;
                        _item.PrefabPath = "Resources/Prefab/Weapon/Prototype_Shotgun";
                        break;
                    case 3:
                        _sectionName = "Prototype_SniperRifle";
                        _item.WeaponName = "Prototype SniperRifle";
                        _item.WeaponType = 0; // TODO: ALTERAR PARA ENUMERADOR!!!
                        _item.Automatic = false;
                        _item.ProjectileID = 4;
                        _item.AmmoMax = 12;
                        _item.Damage = 40;
                        _item.FirePerSecond = 3.5f;
                        _item.ReloadTimeInSeconds = 2.16f;
                        _item.PrefabPath = "Resources/Prefab/Weapon/Prototype_SniperRifle";
                        break;
                    case 4:
                        _sectionName = "Prototype_Pistol";
                        _item.WeaponName = "Prototype Pistol";
                        _item.WeaponType = 2; // TODO: ALTERAR PARA ENUMERADOR!!!
                        _item.Automatic = false;
                        _item.ProjectileID = 1;
                        _item.AmmoMax = 16;
                        _item.Damage = 15;
                        _item.FirePerSecond = 12f;
                        _item.ReloadTimeInSeconds = 0.7f;
                        _item.PrefabPath = "Resources/Prefab/Weapon/Prototype_Pistol";
                        break;
                }

                Section _newSection = Section.FromObject(_sectionName, _item);
                WeaponConfiguration.Add(_newSection);
            }

            WeaponConfiguration.SaveToFile(PathWeaponConfigFile);
        }
    }

    private void InitializeSkillConfiguration()
    {
        if (File.Exists(PathSkillConfigFile))
            SkillConfiguration = SharpConfig.Configuration.LoadFromFile(PathSkillConfigFile);
        else
        {
            SkillConfiguration = new Configuration();
            SkillConfiguration.SaveToFile(PathSkillConfigFile);             
        }
    }

    private void InitializeEnemyConfiguration()
    {
            if (File.Exists(PathEnemyConfigFile))
            EnemyConfiguration = SharpConfig.Configuration.LoadFromFile(PathEnemyConfigFile);
        else
        {
            EnemyConfiguration = new Configuration();
            EnemyConfiguration.SaveToFile(PathEnemyConfigFile);
        }
    }

    private void InitializeQuestConfiguration()
    {
        if (File.Exists(PathQuestConfigFile))
            QuestConfiguration = SharpConfig.Configuration.LoadFromFile(PathQuestConfigFile);
        else
        {
            QuestConfiguration = new Configuration();
            QuestConfiguration.SaveToFile(PathQuestConfigFile);
        }
    }

    private void InitializeCharacterConfiguration()
    {
        if (File.Exists(PathCharacterConfigFile))
            CharacterConfiguration = SharpConfig.Configuration.LoadFromFile(PathCharacterConfigFile);
        else
        {
            CharacterConfiguration = new Configuration();
            CharacterConfiguration.SaveToFile(PathCharacterConfigFile);
        }
    }
}