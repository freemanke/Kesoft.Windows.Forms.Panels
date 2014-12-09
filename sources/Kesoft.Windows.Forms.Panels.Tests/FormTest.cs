using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Kesoft.Windows.Forms.Panels.Tests
{
    public partial class FormTest : Form
    {
        private ToolStripRenderer currentRenderer;
        private ProfessionalColorTable currentColorTable;

        public FormTest()
        {
            InitializeComponent();

            tsbTheme.Click += tsbTheme_Click;
            cbbRenderer.SelectedIndexChanged += cbbRenderer_SelectedValueChanged;
            cbbRenderer.SelectedIndexChanged += cbbColorTable_SelectedIndexChanged;

            var assembly = Assembly.GetAssembly(typeof (Office2007BlackColorTable));
            var typeOfClass = typeof (ToolStripProfessionalRenderer);
            foreach (var type in  assembly.GetTypes())
                if (type.IsClass && typeOfClass.IsAssignableFrom(type))
                    cbbRenderer.Items.Add(type);

            cbbRenderer.Items.Add(typeof (ToolStripProfessionalRenderer));
            cbbRenderer.SelectedItem = typeof (Office2007Renderer);
            var renderer = Activator.CreateInstance(typeof (Office2007Renderer)) as ToolStripProfessionalRenderer;
            ToolStripManager.Renderer = renderer;
        }

        private void cbbColorTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            var type = cbbRenderer.SelectedItem as Type;
            if (type != null)
            {
                var colorTable = Activator.CreateInstance(type) as ProfessionalColorTable;
                if (colorTable != null)
                {
                    if (colorTable.Equals(currentColorTable) == false)
                    {
                        var panelColorTable = colorTable.PanelColorTable;
                        if (panelColorTable != null)
                        {
                            PanelSettingsManager.SetPanelProperties(
                                Controls,
                                panelColorTable);
                        }

                        currentColorTable = colorTable;
                        currentRenderer = Activator.CreateInstance(currentRenderer.GetType(), new object[] { colorTable }) as ToolStripProfessionalRenderer;
                        ToolStripManager.Renderer = currentRenderer;
                    }
                }
            }
        }
        private void cbbRenderer_SelectedValueChanged(object sender, EventArgs e)
        {
            var type = cbbRenderer.SelectedItem as Type;
            if (type != null)
            {
                var toolStripRenderer = Activator.CreateInstance(type) as ToolStripProfessionalRenderer;
                if (toolStripRenderer != null)
                {
                    if (toolStripRenderer.Equals(currentRenderer) == false)
                    {
                        cbbRenderer.Items.Clear();
                        var baseType = toolStripRenderer.ColorTable.GetType().BaseType;
                        if (baseType != null)
                        {
                            var assembly = toolStripRenderer.ColorTable.GetType().Assembly;

                            var types = assembly.GetTypes();

                            foreach (var colorTableType in types)
                            {
                                if (colorTableType.IsClass &&
                                    baseType.IsAssignableFrom(colorTableType) &&
                                    (baseType != colorTableType) &&
                                    (baseType.BaseType == typeof(ProfessionalColorTable)))
                                {
                                    cbbRenderer.Items.Add(colorTableType);
                                }
                            }
                        }

                        var colorTable = toolStripRenderer.ColorTable as ProfessionalColorTable;
                        if (colorTable != null)
                        {
                            var panelColorTable = colorTable.PanelColorTable;
                            if (panelColorTable != null)
                            {
                                PanelSettingsManager.SetPanelProperties(
                                    Controls,
                                    panelColorTable);
                            }
                        }
                        else
                        {
                            var panelColorTable = new PanelColors();
                            PanelSettingsManager.SetPanelProperties(
                                Controls,
                                panelColorTable);
                        }

                        currentRenderer = toolStripRenderer;
                        ToolStripManager.Renderer = currentRenderer;
                    }
                }
            }
        }

        private void tsbTheme_Click(object sender, EventArgs e)
        {
            var renderer = ToolStripManager.Renderer as ToolStripProfessionalRenderer;
            if (renderer != null)
            {
                var colorTable = (ProfessionalColorTable)renderer.ColorTable;
                if (colorTable != null)
                {
                    colorTable.UseSystemColors = !colorTable.UseSystemColors;
                    renderer = Activator.CreateInstance(renderer.GetType(), new object[] {colorTable}) as ToolStripProfessionalRenderer;

                    var panelColors = colorTable.PanelColorTable;
                    if (panelColors != null)
                    {
                        panelColors.UseSystemColors = colorTable.UseSystemColors;
                        PanelSettingsManager.SetPanelProperties(
                            Controls,
                            panelColors);
                    }

                    ToolStripManager.Renderer = renderer;
                }
            }
        }
    }}
