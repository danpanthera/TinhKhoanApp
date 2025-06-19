/**
 * KPI Scoring Service - Frontend API client
 * Handles communication with backend KPI scoring endpoints
 */

const API_BASE = 'https://localhost:7241/api';

/**
 * Mock data for testing scoring flow
 */
const mockKpiTargets = [
  // QUALITATIVE KPIs (chấm điểm thủ công)
  {
    id: 1,
    employeeId: 1,
    employeeName: 'Nguyễn Văn A',
    kpiDefinitionId: 1,
    kpiName: 'Thái độ phục vụ khách hàng',
    kpiType: 'QUALITATIVE',
    valueType: 'PERCENTAGE',
    unit: '%',
    targetValue: 100,
    actualValue: null,
    score: null,
    maxScore: 10,
    period: '2024-12'
  },
  {
    id: 2,
    employeeId: 1,
    employeeName: 'Nguyễn Văn A',
    kpiDefinitionId: 2,
    kpiName: 'Tinh thần làm việc nhóm',
    kpiType: 'QUALITATIVE',
    valueType: 'PERCENTAGE',
    unit: '%',
    targetValue: 100,
    actualValue: null,
    score: null,
    maxScore: 8,
    period: '2024-12'
  },
  
  // QUANTITATIVE_RATIO KPIs (tính toán tỷ lệ)
  {
    id: 3,
    employeeId: 2,
    employeeName: 'Trần Thị B',
    kpiDefinitionId: 3,
    kpiName: 'Tỷ lệ nợ xấu',
    kpiType: 'QUANTITATIVE_RATIO',
    valueType: 'PERCENTAGE',
    unit: '%',
    targetValue: 2.0,
    actualValue: null,
    score: null,
    maxScore: 15,
    period: '2024-12',
    // Additional fields for ratio calculation
    numerator: null,
    denominator: null
  },
  {
    id: 4,
    employeeId: 2,
    employeeName: 'Trần Thị B',
    kpiDefinitionId: 4,
    kpiName: 'Tỷ lệ thực thu lãi',
    kpiType: 'QUANTITATIVE_RATIO',
    valueType: 'PERCENTAGE',
    unit: '%',
    targetValue: 95.0,
    actualValue: null,
    score: null,
    maxScore: 12,
    period: '2024-12',
    numerator: null,
    denominator: null
  },
  
  // QUANTITATIVE_ABSOLUTE KPIs (định lượng tuyệt đối)
  {
    id: 5,
    employeeId: 3,
    employeeName: 'Lê Văn C',
    kpiDefinitionId: 5,
    kpiName: 'Doanh số bán hàng',
    kpiType: 'QUANTITATIVE_ABSOLUTE',
    valueType: 'NUMBER',
    unit: 'triệu VNĐ',
    targetValue: 500,
    actualValue: null,
    score: null,
    maxScore: 20,
    period: '2024-12'
  },
  {
    id: 6,
    employeeId: 3,
    employeeName: 'Lê Văn C',
    kpiDefinitionId: 6,
    kpiName: 'Số khách hàng mới',
    kpiType: 'QUANTITATIVE_ABSOLUTE',
    valueType: 'NUMBER',
    unit: 'khách hàng',
    targetValue: 50,
    actualValue: null,
    score: null,
    maxScore: 15,
    period: '2024-12'
  }
];

/**
 * Get all KPI targets grouped by type
 */
export const getKpiTargetsByType = async () => {
  try {
    // Simulate API call delay
    await new Promise(resolve => setTimeout(resolve, 500));
    
    const groupedTargets = {
      qualitative: mockKpiTargets.filter(t => t.kpiType === 'QUALITATIVE'),
      quantitativeRatio: mockKpiTargets.filter(t => t.kpiType === 'QUANTITATIVE_RATIO'),
      quantitativeAbsolute: mockKpiTargets.filter(t => t.kpiType === 'QUANTITATIVE_ABSOLUTE')
    };
    
    return {
      success: true,
      data: groupedTargets,
      summary: {
        total: mockKpiTargets.length,
        qualitative: groupedTargets.qualitative.length,
        quantitativeRatio: groupedTargets.quantitativeRatio.length,
        quantitativeAbsolute: groupedTargets.quantitativeAbsolute.length,
        scored: mockKpiTargets.filter(t => t.score !== null).length,
        pending: mockKpiTargets.filter(t => t.score === null).length
      }
    };
  } catch (error) {
    console.error('Error fetching KPI targets:', error);
    return {
      success: false,
      error: error.message,
      data: { qualitative: [], quantitativeRatio: [], quantitativeAbsolute: [] }
    };
  }
};

/**
 * Manual scoring for qualitative KPIs
 */
export const submitManualScore = async (targetId, scorePercentage, comments = '') => {
  try {
    const target = mockKpiTargets.find(t => t.id === targetId);
    if (!target) {
      throw new Error('KPI target not found');
    }
    
    if (target.kpiType !== 'QUALITATIVE') {
      throw new Error('Manual scoring only available for qualitative KPIs');
    }
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 300));
    
    // Update mock data
    target.actualValue = scorePercentage;
    target.score = (scorePercentage / 100) * target.maxScore;
    target.comments = comments;
    target.scoredAt = new Date().toISOString();
    
    return {
      success: true,
      message: 'Chấm điểm thành công',
      data: {
        targetId,
        actualValue: target.actualValue,
        score: target.score,
        maxScore: target.maxScore
      }
    };
  } catch (error) {
    console.error('Error submitting manual score:', error);
    return {
      success: false,
      error: error.message
    };
  }
};

/**
 * Calculate ratio-based KPIs
 */
export const calculateRatioScore = async (targetId, numerator, denominator) => {
  try {
    const target = mockKpiTargets.find(t => t.id === targetId);
    if (!target) {
      throw new Error('KPI target not found');
    }
    
    if (target.kpiType !== 'QUANTITATIVE_RATIO') {
      throw new Error('Ratio calculation only available for ratio-based KPIs');
    }
    
    if (denominator === 0) {
      throw new Error('Denominator cannot be zero');
    }
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 400));
    
    // Calculate ratio
    const ratio = (numerator / denominator) * 100;
    
    // Calculate score based on target comparison
    let scorePercentage;
    if (target.kpiName.includes('nợ xấu')) {
      // Lower is better for bad debt ratio
      scorePercentage = ratio <= target.targetValue ? 100 : Math.max(0, 100 - ((ratio - target.targetValue) / target.targetValue * 50));
    } else {
      // Higher is better for other ratios
      scorePercentage = ratio >= target.targetValue ? 100 : (ratio / target.targetValue) * 100;
    }
    
    // Update mock data
    target.numerator = numerator;
    target.denominator = denominator;
    target.actualValue = ratio;
    target.score = (scorePercentage / 100) * target.maxScore;
    target.calculatedAt = new Date().toISOString();
    
    return {
      success: true,
      message: 'Tính toán tỷ lệ thành công',
      data: {
        targetId,
        numerator,
        denominator,
        ratio: ratio.toFixed(2),
        targetValue: target.targetValue,
        scorePercentage: scorePercentage.toFixed(1),
        score: target.score.toFixed(2),
        maxScore: target.maxScore
      }
    };
  } catch (error) {
    console.error('Error calculating ratio score:', error);
    return {
      success: false,
      error: error.message
    };
  }
};

/**
 * Bulk import for absolute KPIs
 */
export const bulkImportScores = async (importData) => {
  try {
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 800));
    
    const results = {
      successful: [],
      failed: [],
      skipped: []
    };
    
    importData.forEach(item => {
      const target = mockKpiTargets.find(t => 
        t.employeeId === item.employeeId && 
        t.kpiDefinitionId === item.kpiDefinitionId
      );
      
      if (!target) {
        results.failed.push({
          ...item,
          reason: 'KPI target not found'
        });
        return;
      }
      
      if (target.kpiType !== 'QUANTITATIVE_ABSOLUTE') {
        results.failed.push({
          ...item,
          reason: 'Not an absolute quantitative KPI'
        });
        return;
      }
      
      // Calculate score
      const achievementRate = (item.actualValue / target.targetValue) * 100;
      const scorePercentage = Math.min(100, achievementRate);
      
      // Update mock data
      target.actualValue = item.actualValue;
      target.score = (scorePercentage / 100) * target.maxScore;
      target.importedAt = new Date().toISOString();
      
      results.successful.push({
        ...item,
        targetId: target.id,
        score: target.score,
        achievementRate: achievementRate.toFixed(1)
      });
    });
    
    return {
      success: true,
      message: `Import completed: ${results.successful.length} successful, ${results.failed.length} failed`,
      data: results
    };
  } catch (error) {
    console.error('Error bulk importing scores:', error);
    return {
      success: false,
      error: error.message
    };
  }
};

/**
 * Get scoring statistics
 */
export const getScoringStatistics = async () => {
  try {
    await new Promise(resolve => setTimeout(resolve, 200));
    
    const stats = {
      totalTargets: mockKpiTargets.length,
      scoredTargets: mockKpiTargets.filter(t => t.score !== null).length,
      pendingTargets: mockKpiTargets.filter(t => t.score === null).length,
      averageScore: 0,
      totalMaxScore: mockKpiTargets.reduce((sum, t) => sum + t.maxScore, 0),
      actualTotalScore: mockKpiTargets.reduce((sum, t) => sum + (t.score || 0), 0)
    };
    
    const scoredTargets = mockKpiTargets.filter(t => t.score !== null);
    if (scoredTargets.length > 0) {
      stats.averageScore = scoredTargets.reduce((sum, t) => sum + (t.score / t.maxScore) * 100, 0) / scoredTargets.length;
    }
    
    return {
      success: true,
      data: stats
    };
  } catch (error) {
    console.error('Error fetching statistics:', error);
    return {
      success: false,
      error: error.message,
      data: {}
    };
  }
};

// Export mock data for testing
export const mockData = {
  kpiTargets: mockKpiTargets
};
