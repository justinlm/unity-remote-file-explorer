using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace RemoteFileExplorer.Editor.UI 
{
    public class ObjectListArea : ScrollView 
    {
        private VerticalGrid m_Grid = new VerticalGrid();

        private List<ObjectItem> m_Items = new List<ObjectItem>();

        private List<ObjectData> m_Data = new List<ObjectData>();

        private VisualElement m_Content;

        private ObjectItem m_CurSelectItem;

        public Action<ObjectItem> clickItemCallback;
        public Action<ObjectItem> doubleClickItemCallback;

        public ObjectListArea() : base(ScrollViewMode.Vertical)
        {
            this.style.width = Length.Percent(100);
            this.style.height = Length.Percent(100);

            m_Content = new VisualElement();
            Add(m_Content);

            m_Grid.itemSize = new Vector2(80, 80);
            m_Grid.minHorizontalSpacing = 10;
            m_Grid.verticalSpacing = 10;
            m_Grid.topMargin = 10;
        }

        public void UpdateView(List<ObjectData> list)
        {
            m_Content.Clear();
            m_CurSelectItem = null;
            m_Data = list;

            m_Grid.fixedWidth = this.contentRect.width;
            m_Grid.InitNumRowsAndColumns(list.Count);
            m_Content.style.width = m_Grid.fixedWidth;
            m_Content.style.height = m_Grid.height;


            for(int i = 0; i < list.Count; i ++)
            {
                Rect rect = m_Grid.CalcRect(i);
                var item = new ObjectItem(m_Grid.itemSize);
                item.style.position = Position.Absolute;
                item.style.marginLeft = rect.x;
                item.style.marginTop = rect.y;

                item.clickItemCallback += OnClickItem;
                item.doubleClickItemCallback += OnDoubleClickItem;
                
                item.UpdateView(m_Data[i]);
                m_Items.Add(item);
                m_Content.Add(item);
            }
        }

        public void AddRow(int cols, int row)
        {
            VisualElement v = new VisualElement();
            v.style.width = Length.Percent(100);
            v.style.flexDirection = FlexDirection.Row;
            v.style.marginTop = m_Grid.verticalSpacing;

            for(int i = 0; i < cols; i ++)
            {
                int index = row * cols + i;
                if(index >= m_Data.Count)
                {
                    break;
                }
                var item = new ObjectItem(m_Grid.itemSize);
                item.clickItemCallback += OnClickItem;
                item.doubleClickItemCallback += OnDoubleClickItem;
                v.Add(item);
                item.UpdateView(m_Data[index]);
                m_Items.Add(item);
            }
            Add(v);
        }

        public void OnClickItem(ObjectItem item)
        {
            if(m_CurSelectItem != null && m_CurSelectItem != item)
            {
                m_CurSelectItem.UpdateState(ObjectState.Normal);
            }
            m_CurSelectItem = item;
            m_CurSelectItem.UpdateState(ObjectState.Selected);
            if(clickItemCallback != null)
            {
                clickItemCallback(item);
            }
        }

        public void OnDoubleClickItem(ObjectItem item)
        {
            if(doubleClickItemCallback != null)
            {
                doubleClickItemCallback(item);
            }
        }
    }

    internal class VerticalGrid
    {
        int m_Columns = 1;
        int m_Rows;
        float m_Height;
        float m_HorizontalSpacing;

        public int columns {get {return m_Columns;}}
        public int rows {get {return m_Rows;}}
        public float height {get {return m_Height;}}
        public float horizontalSpacing {get {return m_HorizontalSpacing;}}

        public float fixedWidth {get; set;}
        public Vector2 itemSize {get; set;}
        public float verticalSpacing {get; set;}
        public float minHorizontalSpacing {get; set;}
        public float topMargin {get; set;}
        public float bottomMargin {get; set;}
        public float rightMargin {get; set;}
        public float leftMargin {get; set;}
        public float fixedHorizontalSpacing {get; set;}
        public bool useFixedHorizontalSpacing {get; set;}

        public void InitNumRowsAndColumns(int itemCount)
        {
            m_Columns = CalcColumns();
            m_Rows = CalcRows(itemCount);
            if(useFixedHorizontalSpacing)
            {
                m_HorizontalSpacing = fixedHorizontalSpacing;
            }
            else
            {
                m_HorizontalSpacing = Mathf.Max(0f, (fixedWidth - (m_Columns * itemSize.x + leftMargin + rightMargin)) / (m_Columns));
                if(m_Rows == 1)
                {
                    m_HorizontalSpacing = minHorizontalSpacing;
                }
            }
            m_Height = m_Rows * (itemSize.y + verticalSpacing) - verticalSpacing + topMargin + bottomMargin;
        }

        public int CalcColumns()
        {
            float horizontalSpacing = useFixedHorizontalSpacing ? fixedHorizontalSpacing : minHorizontalSpacing;
            int cols = (int)Mathf.Floor((fixedWidth - leftMargin - rightMargin) / (itemSize.x + horizontalSpacing));
            cols = Mathf.Max(cols, 1);
            return cols;
        }

        public int CalcRows(int itemCount)
        {
            return (int)Mathf.Ceil(itemCount / (float)CalcColumns());
        }

        public Rect CalcRect(int itemIdx)
        {
            float row = Mathf.Floor(itemIdx / columns);
            float column = itemIdx - row * columns;
            if(useFixedHorizontalSpacing)
            {
                return new Rect(leftMargin + column * (itemSize.x + fixedHorizontalSpacing),
                    row * (itemSize.y + verticalSpacing) + topMargin,
                    itemSize.x,
                    itemSize.y);
            }
            else
            {
                return new Rect(leftMargin + horizontalSpacing * 0.5f +  column * (itemSize.x + horizontalSpacing),
                    row * (itemSize.y + verticalSpacing) + topMargin,
                    itemSize.x,
                    itemSize.y);
            }
        }
    }
}