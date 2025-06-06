#!/bin/bash

# AI Logica - Manual Testing Script
# This script helps test the OR gate selection functionality

echo "🔧 AI Logica - Manual Testing Guide"
echo "=================================="
echo ""

echo "📋 Testing Checklist for OR Gate Selection:"
echo ""
echo "1. Build and start the application:"
echo "   dotnet run --project AiLogica"
echo ""
echo "2. Open your browser to: http://localhost:5000"
echo ""
echo "3. Test the following scenarios:"
echo ""
echo "   ✅ OR Gate Selection:"
echo "      - Click the 'OR' button in the Gate Palette (left panel)"
echo "      - The OR button should turn blue (highlighted)"
echo "      - Check Properties panel: Should show 'Selected: OR' and 'Status: Dragging'"
echo "      - Debug info should show: SelectedGate=OR, IsDragging=True"
echo ""
echo "   ✅ Mouse Movement:"
echo "      - Move mouse over the main canvas area"
echo "      - A semi-transparent OR gate should follow your cursor"
echo ""
echo "   ✅ Gate Placement:"
echo "      - Click anywhere on the canvas"
echo "      - The OR gate should be placed at that location"
echo "      - Properties panel should show 'No gate selected'"
echo "      - Status bar should show 'Gates: 1'"
echo ""
echo "   ✅ Selection Reset:"
echo "      - Click OR again"
echo "      - Move mouse outside canvas area"
echo "      - Selection should be cancelled"
echo ""
echo "🧪 Run Automated Tests:"
echo "   dotnet test"
echo ""
echo "📊 Expected Results:"
echo "   - Unit tests: 8/8 passing (ViewModel tests)"
echo "   - UI tests: 2/2 passing (Component interaction)"
echo "   - Integration tests: 2/2 passing (Application startup)"
echo ""
echo "🔍 If OR gate selection is not working:"
echo "   1. Check if the OR button shows a checkmark (✓) when clicked"
echo "   2. Look at the Debug info in the Properties panel"
echo "   3. Open browser developer tools and check for JavaScript errors"
echo "   4. Verify the build completed without warnings"
echo ""
echo "📝 Report Issues:"
echo "   Please include:"
echo "   - Browser type and version"
echo "   - Any JavaScript console errors"
echo "   - Screenshots of the Properties panel debug info"
echo "   - Test results output"
echo ""