using Diplomata;
using Diplomata.Dictionaries;
using Diplomata.Helpers;
using Diplomata.Models;
using DiplomataEditor;
using DiplomataEditor.Helpers;
using UnityEditor;
using UnityEngine;

namespace DiplomataEditor.Windows
{
  public class ContextListMenu
  {
    private static Vector2 scrollPos = new Vector2(0, 0);

    public static void Draw()
    {
      var diplomataEditor = TalkableMessagesManager.diplomataEditor;
      var talkable = TalkableMessagesManager.talkable;
      var listWidth = Screen.width / 3;
      string folderName = (talkable.GetType() == typeof(Character)) ? "Characters" : "Interactables";

      if (talkable != null)
      {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.BeginHorizontal();
        GUILayout.Space(listWidth);

        GUILayout.BeginVertical(GUIHelper.windowStyle, GUILayout.Width(listWidth));

        if (EditorGUIUtility.isProSkin)
        {
          GUIHelper.labelStyle.normal.textColor = GUIHelper.proTextColor;
        }

        else
        {
          GUIHelper.labelStyle.normal.textColor = GUIHelper.freeTextColor;
        }

        GUIHelper.labelStyle.fontSize = 24;
        GUIHelper.labelStyle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label(talkable.name, GUIHelper.labelStyle, GUILayout.Height(50));

        for (int i = 0; i < talkable.contexts.Length; i++)
        {

          var context = (Context) Find.In(talkable.contexts).Where("id", i).Result;

          Rect boxRect = EditorGUILayout.BeginVertical(GUIHelper.boxStyle);
          GUI.Box(boxRect, GUIContent.none);

          var name = DictionariesHelper.ContainsKey(context.name, diplomataEditor.options.currentLanguage);

          if (name == null)
          {
            context.name = ArrayHelper.Add(context.name, new LanguageDictionary(diplomataEditor.options.currentLanguage, "Name [Change clicking on Edit]"));
            name = DictionariesHelper.ContainsKey(context.name, diplomataEditor.options.currentLanguage);
          }

          var description = DictionariesHelper.ContainsKey(context.description, diplomataEditor.options.currentLanguage);

          if (description == null)
          {
            context.description = ArrayHelper.Add(context.description, new LanguageDictionary(diplomataEditor.options.currentLanguage, "Description [Change clicking on Edit]"));
            description = DictionariesHelper.ContainsKey(context.description, diplomataEditor.options.currentLanguage);
          }

          GUIHelper.labelStyle.fontSize = 11;
          GUIHelper.labelStyle.alignment = TextAnchor.UpperCenter;

          GUIHelper.textContent.text = "<size=13><i>" + name.value + "</i></size>\n\n" + description.value + "\n";
          var height = GUIHelper.labelStyle.CalcHeight(GUIHelper.textContent, listWidth);

          GUILayout.Label(GUIHelper.textContent, GUIHelper.labelStyle, GUILayout.Width(listWidth), GUILayout.Height(height));

          GUILayout.BeginHorizontal();
          if (GUILayout.Button("Edit", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
          {
            ContextEditor.Edit(talkable, context);
          }

          if (GUILayout.Button("Edit Messages", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
          {
            TalkableMessagesManager.OpenMessagesMenu(talkable, context);
          }

          if (GUILayout.Button("Delete", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
          {
            if (EditorUtility.DisplayDialog("Are you sure?", "All data inside this context will be lost forever.", "Yes", "No"))
            {
              ContextEditor.Reset(talkable.name);
              talkable.contexts = ArrayHelper.Remove(talkable.contexts, context);
              talkable.contexts = Context.ResetIDs(talkable, talkable.contexts);
              diplomataEditor.Save(talkable, folderName);
            }
          }

          if (GUILayout.Button("Move Up", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
          {
            if (context.id > 0)
            {
              var neighboorContext = (Context) Find.In(talkable.contexts).Where("id", context.id - 1).Result;

              neighboorContext.id += 1;
              context.id -= 1;

              diplomataEditor.Save(talkable, folderName);
            }
          }

          if (GUILayout.Button("Move Down", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
          {
            if (context.id < talkable.contexts.Length - 1)
            {
              var neighboorContext = (Context) Find.In(talkable.contexts).Where("id", context.id + 1).Result;
              neighboorContext.id -= 1;
              context.id += 1;

              diplomataEditor.Save(talkable, folderName);
            }
          }

          GUILayout.EndHorizontal();

          EditorGUILayout.EndVertical();

          EditorGUILayout.Separator();
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Context", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_BIG)))
        {
          CreateContext();
        }

        if (GUILayout.Button("Save as Screenplay", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_BIG)))
        {
          if (EditorUtility.DisplayDialog("This character only?", "This character only or all characters?", "Only this character", "All characters"))
          {
            Screenplay.Save(talkable);
          }
          else
          {
            Screenplay.SaveAll();
          }
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
      }

      else
      {
        GUILayout.BeginVertical(GUIHelper.windowStyle);
        EditorGUILayout.HelpBox("This characters doesn't exist anymore.", MessageType.Info);
        GUILayout.EndVertical();
      }
    }

    public static void CreateContext()
    {
      var talkable = TalkableMessagesManager.talkable;

      talkable.contexts = ArrayHelper.Add(talkable.contexts, new Context(talkable.contexts.Length, talkable.name));
      string folderName = (talkable.GetType() == typeof(Character)) ? "Characters" : "Interactables";
      TalkableMessagesManager.diplomataEditor.Save(talkable, folderName);
    }

  }

}
